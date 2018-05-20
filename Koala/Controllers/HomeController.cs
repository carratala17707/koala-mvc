using Koala.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace Koala.Controllers
{
    public class HomeController : BaseController
    {
        public async Task<ActionResult> Index()
        {
            var tipos = await _db.Tipo_Producto.ToListAsync();
            return View(tipos);
        }

        [Authorize]
        public ActionResult About()
        {
            var nombreUsuarioLogueado = User.Identity.Name;
            if (User.IsInRole(KoalaRoles.UserCliente))
            {
                ViewBag.Message = "Soy un cliente.";
            }
            else
            {
            }

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Policy()
        {
            return View();
        }
    }
}