using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Koala.Controllers
{
    public class ErrorController : BaseController
    {
        public ActionResult Index()
        {
            return RedirectToAction("PageNotFound", "Error");
        }

        public ActionResult PageNotFound()
        {
            Response.StatusCode = 404;
            return View();
        }

        public ActionResult InternalServerError()
        {
            //Response.StatusCode = 500;
            return View();
        }

        public ActionResult Forbidden()
        {
            //Response.StatusCode = 403;
            return View();
        }
    }
}