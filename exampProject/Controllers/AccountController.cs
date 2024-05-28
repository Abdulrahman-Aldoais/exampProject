using exampProject.Models;
using exampProject.Models.ViewModel;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text.Encodings.Web;


namespace exampProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _sinInManager;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _sinInManager = signInManager;
        }

        public class LoginViewModelValidator : AbstractValidator<LoginVM>
        {
            public LoginViewModelValidator()
            {
                RuleFor(x => x.Email).NotEmpty().EmailAddress();
                RuleFor(x => x.Password).NotEmpty();
            }
        }


        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public async Task<ActionResult> Regester()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Regester(RegesterVM registerVM)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(registerVM.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "This email already exists.");
                    return View(registerVM);
                }

                var user = new User
                {
                    Email = registerVM.Email,
                    UserName = registerVM.Email,
                    Name = registerVM.Name,
                    EmailConfirmed = true,

                    PhoneNumberConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, registerVM.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User"); // Assign role
                    await _sinInManager.SignInAsync(user, isPersistent: false); // Correct variable name

                    var claim = new Claim("Name", registerVM.Name); // Create a custom claim
                    await _userManager.AddClaimAsync(user, claim); // Add claim to the correct user object

                    return RedirectToAction("Login", "Account"); // Redirect to login page after registration
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description); // Add error descriptions to ModelState
                    }
                }
            }
            return View(registerVM);
        }




        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View();

        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (ModelState.IsValid)
            {
                var getUserForCheck = await _userManager.FindByEmailAsync(loginVM.Email);
                if (getUserForCheck != null)
                {
                    var checkPassword = await _sinInManager.CheckPasswordSignInAsync(getUserForCheck, loginVM.Password, false);
                    if (checkPassword.Succeeded)
                    {
                        // var userClaims = await _userManager.GetClaimsAsync(getUserForCheck);

                        return RedirectToAction("Index", "Home");
                    }
                }
                ModelState.AddModelError("login field", "your password not corect");
                return View(loginVM);
            }
            return View();
        }


        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Handle user not found scenario
                return View("Error");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Account", new { UserId = user.Id, token = token }, protocol: HttpContext.Request.Scheme);
            await SendEmail(user.Email, "إستعادة كلمة المرور", $"قم بالضغظ على الرابط لاستعادة كلمة المرور <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>here</a>.");
            return View("ForgotPassword");
        }


        [HttpPost]

        public async Task SendEmail(string to, string subject, string body)
        {
            var message = new MailMessage("yemenblog9@gmail.com", to, subject, body)
            {
                IsBodyHtml = true
            };

            using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
            {
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential("yemenblog9@gmail.com", "jihezeaeelidmvpj");
                try
                {
                    await smtpClient.SendMailAsync(message);
                }
                catch (SmtpException ex)
                {
                    throw new InvalidOperationException("Failed to send email.", ex);
                }
            }
        }


        [HttpGet]
        public IActionResult ResetPassword(string userId, string token)
        {
            if (token == null || userId == null)
            {
                ModelState.AddModelError("", "Invalid password reset token.");
            }
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user != null)
            {
                var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Login", "Account");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }



        [HttpGet]
        public async Task<IActionResult> Logout()
        {

            var signOutTask = _sinInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }




    }
}
