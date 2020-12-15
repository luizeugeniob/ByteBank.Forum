using ByteBank.Forum.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Linq;

namespace ByteBank.Forum.ViewModels
{
    public class UserEditRolesViewModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }

        public List<UserRoleViewModel> UserRoles { get; set; }

        public UserEditRolesViewModel() { }

        public UserEditRolesViewModel(ApplicationUser user, RoleManager<IdentityRole> roleManager)
        {
            Id = user.Id;
            FullName = user.FullName;
            Email = user.Email;
            UserName = user.UserName;

            UserRoles =
                roleManager
                    .Roles
                    .ToList()
                    .Select(role => new UserRoleViewModel
                    {
                        Id = role.Id,
                        Name = role.Name
                    })
                    .ToList();

            foreach (var role in UserRoles)
                role.Selected = user.Roles.Any(userRole => userRole.RoleId == role.Id);
        }
    }
}