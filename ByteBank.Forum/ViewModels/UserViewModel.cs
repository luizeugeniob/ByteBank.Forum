using ByteBank.Forum.Models;

namespace ByteBank.Forum.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }

        public UserViewModel() { }

        public UserViewModel(ApplicationUser applicationUser)
        {
            Id = applicationUser.Id;
            FullName = applicationUser.FullName;
            Email = applicationUser.Email;
            UserName = applicationUser.UserName;
        }
    }
}