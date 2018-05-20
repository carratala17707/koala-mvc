using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Linq;

namespace Koala.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }

    public class IdentityManager
    {
        public async Task<bool> RoleExistsAsync(string name)
        {
            using (var db = new ApplicationDbContext())
            {
                var rm = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
                return await rm.RoleExistsAsync(name);
            }
        }

        public async Task<bool> UserExistsAsync(string name)
        {
            using (var db = new ApplicationDbContext())
            {
                var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                return await um.Users.Where(u => u.UserName == name).AnyAsync();
            }
        }

        public async Task<bool> CreateRoleAsync(string name)
        {
            using (var db = new ApplicationDbContext())
            {
                var rm = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
                var role = await rm.FindByNameAsync(name);
                if (role == null)
                {
                    role = new IdentityRole(name);
                    var idResult = await rm.CreateAsync(new IdentityRole(name));
                    return idResult.Succeeded;
                }
                return true;
            }
        }

        public async Task<bool> CreateUserAsync(ApplicationUser user, string password)
        {
            using (var db = new ApplicationDbContext())
            {
                var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                if (await um.FindByNameAsync(user.UserName) == null)
                {
                    var idResult = await um.CreateAsync(user, password);
                    return idResult.Succeeded;
                }
                return true;
            }
        }

        public async Task<bool> AddUserToRoleAsync(string userId, string roleName)
        {
            using (var db = new ApplicationDbContext())
            {
                var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                var rolesForUser = await um.GetRolesAsync(userId);
                if (!rolesForUser.Contains(roleName))
                {
                    var idResult = await um.AddToRoleAsync(userId, roleName);
                    return idResult.Succeeded;
                }
                return true;
            }
        }

        public async Task<bool> RemoveUserFromRoleAsync(string userId, string roleName)
        {
            using (var db = new ApplicationDbContext())
            {
                var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                var idResult = await um.RemoveFromRoleAsync(userId, roleName);
                return idResult.Succeeded;
            }
        }

        public async Task DeleteRoleAsync(string roleId)
        {
            using (var db = new ApplicationDbContext())
            {
                var roleUsers = db.Users.Where(u => u.Roles.Any(r => r.RoleId == roleId));
                var role = db.Roles.Find(roleId);

                foreach (var user in roleUsers)
                {
                    await this.RemoveUserFromRoleAsync(user.Id, role.Name);
                }

                db.Roles.Remove(role);
                db.SaveChanges();
            }
        }
    }
}