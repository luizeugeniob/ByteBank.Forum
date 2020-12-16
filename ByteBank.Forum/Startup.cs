using ByteBank.Forum.App_Start.Identity;
using ByteBank.Forum.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using System;
using System.Configuration;
using System.Data.Entity;

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
                (opcoes, owinContext) =>
                {
                    var dbContext = owinContext.Get<DbContext>();
                    return new UserStore<ApplicationUser>(dbContext);
                });
            
            builder.CreatePerOwinContext<RoleStore<IdentityRole>>(
                (opcoes, owinContext) =>
                {
                    var dbContext = owinContext.Get<DbContext>();
                    return new RoleStore<IdentityRole>(dbContext);
                });
            
            builder.CreatePerOwinContext<RoleManager<IdentityRole>>(
                (opcoes, owinContext) =>
                {
                    var roleStore = owinContext.Get<RoleStore<IdentityRole>>();
                    return new RoleManager<IdentityRole>(roleStore);
                });

            builder.CreatePerOwinContext<UserManager<ApplicationUser>>(
                (options, owinContext) =>
                {
                    var userStore = owinContext.Get<IUserStore<ApplicationUser>>();
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
                    userManager.SmsService = new SmsService();

                    var dataProtectionProvider = options.DataProtectionProvider;
                    var dataProtector = dataProtectionProvider.Create("ByteBank.Forum");

                    userManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtector);

                    userManager.MaxFailedAccessAttemptsBeforeLockout = 3;
                    userManager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    userManager.UserLockoutEnabledByDefault = true;

                    return userManager;
                });

            builder.CreatePerOwinContext<SignInManager<ApplicationUser, string>>(
                (options, owinContext) =>
                {
                    var userManager = owinContext.Get<UserManager<ApplicationUser>>();
                    var signInManager =
                        new SignInManager<ApplicationUser, string>(
                            userManager,
                            owinContext.Authentication);

                    return signInManager;
                });

            builder.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie
            });

            builder.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            builder.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
            {
                ClientId = ConfigurationManager.AppSettings["google:client_id"],
                ClientSecret = ConfigurationManager.AppSettings["google:client_secret"],
                Caption = "Google"
            });

            using (var dbContext = new IdentityDbContext<ApplicationUser>("DefaultConnection"))
            {
                CreateRoles(dbContext);
                CreateAdmin(dbContext);
            }
        }

        private void CreateRoles(IdentityDbContext<ApplicationUser> dbContext)
        {
            using (var roleStore = new RoleStore<IdentityRole>(dbContext))
            using (var roleManager = new RoleManager<IdentityRole>(roleStore))
            {
                if (!roleManager.RoleExists(RolesNames.Admin))
                    roleManager.Create(new IdentityRole(RolesNames.Admin));

                if (!roleManager.RoleExists(RolesNames.Moderator))
                    roleManager.Create(new IdentityRole(RolesNames.Moderator));
            };
        }

        private void CreateAdmin(IdentityDbContext<ApplicationUser> dbContext)
        {
            using (var userStore = new UserStore<ApplicationUser>(dbContext))
            using (var userManager = new UserManager<ApplicationUser>(userStore))
            {
                var adminEmail = ConfigurationManager.AppSettings["admin:email"];
                var adminUser = userManager.FindByEmail(adminEmail);

                if (adminUser != null)
                    return;

                adminUser = new ApplicationUser
                {
                    Email = adminEmail,
                    UserName = ConfigurationManager.AppSettings["admin:username"],
                    EmailConfirmed = true
                };

                userManager.Create(
                    adminUser,
                    ConfigurationManager.AppSettings["admin:password"]);

                userManager.AddToRole(adminUser.Id, RolesNames.Admin);
            }
        }
    }
}