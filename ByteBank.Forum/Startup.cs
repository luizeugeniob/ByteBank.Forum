using ByteBank.Forum.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using System.Data.Entity;
using ByteBank.Forum.App_Start.Identity;

[assembly: OwinStartup(typeof(ByteBank.Forum.Startup))]

namespace ByteBank.Forum
{
    public class Startup
    {
        public void Configuration(IAppBuilder builder)
        {
            builder.CreatePerOwinContext<DbContext>(() =>
                new IdentityDbContext<ApplicationUser>("DefaultConnection"));

            builder.CreatePerOwinContext<IUserStore<ApplicationUser>>(
            (opcoes, contextoOwin) =>
            {
                var dbContext = contextoOwin.Get<DbContext>();
                return new UserStore<ApplicationUser>(dbContext);
            });

            builder.CreatePerOwinContext<UserManager<ApplicationUser>>(
            (options, owinConxt) =>
            {
                var userStore = owinConxt.Get<IUserStore<ApplicationUser>>();
                var userManager = new UserManager<ApplicationUser>(userStore);

                var userValidator = new UserValidator<ApplicationUser>(userManager);
                userValidator.RequireUniqueEmail = true;

                userManager.UserValidator = userValidator;
                userManager.PasswordValidator = new CustomPasswordValidator()
                {
                    LengthRequired = 6,
                    MustHaveSpecialCharacters = true,
                    MustHaveLowerCase = true,
                    MustHaveUpperCase = true,
                    MustHaveNumbers = true
                };

                userManager.EmailService = new EmailService();

                var dataProtectionProvider = options.DataProtectionProvider;
                var dataProtector = dataProtectionProvider.Create("ByteBank.Forum");

                userManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtector);

                return userManager;
            });
        }
    }
}