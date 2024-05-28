using Microsoft.AspNetCore.Identity;

namespace exampProject.Models
{
    public class User : IdentityUser<Guid>
    {

        public string Name { get; set; }
        //public virtual Role Roles { get; set; }
        public virtual ICollection<IdentityRole> Roles { get; set; }

    }
}
