using exampProject.Models;
using Microsoft.AspNetCore.Identity;

namespace exampProject.Seeder
{
    public static class UserSeeder
    {
        public static async Task SeedAsync(UserManager<User> _userManager)
        {

            var users = new List<(User user, string role)>
    {
        (new User
        {
             Id = Guid.Parse("767343b7-422c-4c53-001c-08dc790a6b5d"),
             Name = "حسن محفل",
             UserName = "admin",
             Email = "hassan@gmail.com",
             PhoneNumber = "7777777777",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true
        }, "Admin"),

        (new User
        {
            Id = Guid.Parse("267343b7-422c-4c53-001c-08dc790a6b5d"),
            Name = "صلاح حمود",
             UserName = "user",
             Email = "salah@gmail.com",
             PhoneNumber = "7777777777",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true
        }, "User"),// Role
    };

            foreach (var (user, role) in users)
            {
                var userExists = await _userManager.FindByEmailAsync(user.Email);
                if (userExists == null)
                {

                    var createUserResult = await _userManager.CreateAsync(user, "Pass@123*");
                    if (createUserResult.Succeeded)
                    {


                        var addToRoleResult = await _userManager.AddToRoleAsync(user, role);
                        if (!addToRoleResult.Succeeded)
                        {
                            foreach (var error in addToRoleResult.Errors)
                            {
                                Console.WriteLine($"Error adding to role: {error.Description}");
                            }
                        }

                    }
                    else
                    {

                        foreach (var error in createUserResult.Errors)
                        {
                            Console.WriteLine($"Error creating user: {error.Description}");
                        }
                    }

                }
            }
        }


    }
}

