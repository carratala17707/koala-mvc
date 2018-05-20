using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Koala.Models;

namespace Koala.Controllers
{
    public class ClientesController : BaseController
    {
        // GET: Clientes
        public async Task<ActionResult> Index()
        {
            var clientes = _db.Clientes.Include(c => c.Usuarios);
            return View(await clientes.ToListAsync());
        }

        // GET: Clientes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Clientes clientes = await _db.Clientes.FindAsync(id);
            if (clientes == null)
            {
                return HttpNotFound();
            }
            return View(clientes);
        }

        // GET: Clientes/Create
        public ActionResult Create()
        {
            ViewBag.DNI_Cliente = new SelectList(_db.Usuarios, "DNI", "Nombre");
            return View();
        }

        // POST: Clientes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id_Cliente,DNI_Cliente,Nombre,Apellidos,Estado,Telefono,Email,Direccion,Poblacion,Foto,Nick,Fecha_Nacimiento")] Clientes clientes)
        {
            if (ModelState.IsValid)
            {
                _db.Clientes.Add(clientes);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.DNI_Cliente = new SelectList(_db.Usuarios, "DNI", "Nombre", clientes.DNI_Cliente);
            return View(clientes);
        }

        // GET: Clientes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Clientes clientes = await _db.Clientes.FindAsync(id);
            if (clientes == null)
            {
                return HttpNotFound();
            }
            ViewBag.DNI_Cliente = new SelectList(_db.Usuarios, "DNI", "Nombre", clientes.DNI_Cliente);
            return View(clientes);
        }

        // POST: Clientes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id_Cliente,DNI_Cliente,Nombre,Apellidos,Estado,Telefono,Email,Direccion,Poblacion,Foto,Nick,Fecha_Nacimiento")] Clientes clientes)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(clientes).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.DNI_Cliente = new SelectList(_db.Usuarios, "DNI", "Nombre", clientes.DNI_Cliente);
            return View(clientes);
        }

        // GET: Clientes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Clientes clientes = await _db.Clientes.FindAsync(id);
            if (clientes == null)
            {
                return HttpNotFound();
            }
            return View(clientes);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Clientes clientes = await _db.Clientes.FindAsync(id);
            _db.Clientes.Remove(clientes);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
