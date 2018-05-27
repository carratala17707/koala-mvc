using Koala.Models;
using Microsoft.Owin;
using Owin;
using System.Threading.Tasks;


[assembly: OwinStartupAttribute(typeof(Koala.Startup))]
namespace Koala
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            //CreateAdminUser().Wait();
        }

        private async Task CreateAdminUser()
        {
            var user = new ApplicationUser { UserName = "Admin", Email = "admin@gmail.com" };
            var manager = new IdentityManager();
            await manager.CreateRoleAsync(KoalaRoles.UserAdmin);
            bool success = await manager.CreateUserAsync(user, "123456");
            if (success)
            {
                await manager.AddUserToRoleAsync(user.Id, KoalaRoles.UserAdmin);
            }
        }
    }
}
