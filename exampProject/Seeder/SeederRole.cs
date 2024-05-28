using exampProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace exampProject.Seeder
{
    public static class SeederRole
    {
        public static async Task SeedAsync(RoleManager<Role> _roleManager)
        {
            var roleCount = await _roleManager.Roles.CountAsync();
            if (roleCount <= 0)
            {
                await _roleManager.CreateAsync(new Role
                {
                    Name = "User"
                });


                await _roleManager.CreateAsync(new Role
                {
                    Name = "Admin"
                });

            }
        }
    }
}
