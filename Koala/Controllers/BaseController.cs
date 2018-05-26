using Koala.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Koala.Controllers
{
    public class BaseController : Controller
    {
        protected KoalaEntities _db = new KoalaEntities();
        protected ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }

        protected async Task<Models.Usuarios> GetUser()
        {
            var userId = User.Identity.GetUserId();
            var aspnetUser = await UserManager.FindByIdAsync(userId);
            var user = await _db.Usuarios.Where(u => u.Nick == aspnetUser.UserName)
                .FirstOrDefaultAsync(); 
            return user;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}