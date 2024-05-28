namespace exampProject.Models.ViewModel
{
    public class UserViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public string Role { get; set; }
    }

}
