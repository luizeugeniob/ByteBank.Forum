using Microsoft.AspNet.Identity.EntityFramework;

namespace ByteBank.Forum.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}