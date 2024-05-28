using exampProject.DBContext;
using exampProject.Helpers;
using exampProject.Models;
using exampProject.Repository.Implemntation;
using exampProject.Repository.Interface;
using exampProject.Seeder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();

builder.Services.AddIdentity<User, Role>(option =>
{
    // Password settings.
    option.Password.RequireDigit = true;
    option.Password.RequireLowercase = true;
    option.Password.RequireNonAlphanumeric = true;
    option.Password.RequireUppercase = true;
    option.Password.RequiredLength = 6;
    option.Password.RequiredUniqueChars = 1;

    // Lockout settings.
    option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    option.Lockout.MaxFailedAccessAttempts = 5;
    option.Lockout.AllowedForNewUsers = true;

}).AddEntityFrameworkStores<DbaceContext>().AddDefaultTokenProviders();
var jwtSettings = new JwtSettings();
builder.Configuration.GetSection(nameof(jwtSettings)).Bind(jwtSettings);

builder.Services.AddSingleton(jwtSettings);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
         .AddJwtBearer(x =>
         {
             x.RequireHttpsMetadata = false;
             x.SaveToken = true;
             x.TokenValidationParameters = new TokenValidationParameters
             {
                 ValidateIssuer = jwtSettings.ValidateIssuer,
                 ValidIssuers = new[] { jwtSettings.Issuer },
                 ValidateIssuerSigningKey = jwtSettings.ValidateIssuerSigningKey,
                 IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
                 ValidAudience = jwtSettings.Audience,
                 ValidateAudience = jwtSettings.ValidateAudience,
                 ValidateLifetime = jwtSettings.ValidateLifeTime,
             };
         });

builder.Services.AddAuthorization(config =>
{
    var userAuthPolicyBuilder = new AuthorizationPolicyBuilder();
    config.DefaultPolicy = userAuthPolicyBuilder
                        .RequireAuthenticatedUser()
                        .RequireClaim(ClaimTypes.DateOfBirth)
                        .Build();


});



var connectionString = "Data Source=DESKTOP-ALDOAIS\\SQL;Initial Catalog=example;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;";
builder.Services.AddDbContext<DbaceContext>(options =>
    options.UseSqlServer(connectionString)
           .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
    await SeederRole.SeedAsync(roleManager);
    await UserSeeder.SeedAsync(userManager);
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
