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
    public class HomeController : Controller
    {
        private KoalaEntities db = new KoalaEntities();

        public async Task<ActionResult> Index()
        {
            var tipos = await db.Tipo_Producto.ToListAsync();
            return View(tipos);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}