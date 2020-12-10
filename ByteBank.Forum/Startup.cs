using ByteBank.Forum.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
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
            (opcoes, contextoOwin) =>
            {
                var dbContext = contextoOwin.Get<DbContext>();
                return new UserStore<ApplicationUser>(dbContext);
            });

            builder.CreatePerOwinContext<UserManager<ApplicationUser>>(
            (opcoes, contextoOwin) =>
            {
                var userStore = contextoOwin.Get<IUserStore<ApplicationUser>>();
                return new UserManager<ApplicationUser>(userStore);
            });
        }
    }
}