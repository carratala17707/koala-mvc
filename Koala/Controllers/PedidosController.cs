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
    public class PedidosController : BaseController
    { 
        // GET: Pedidos
        public async Task<ActionResult> Index()
        {
            var pedidos = _db.Pedidos.Include(p => p.Clientes);
            return View(await pedidos.ToListAsync());
        }

        // GET: Pedidos/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pedidos pedidos = await _db.Pedidos.FindAsync(id);
            if (pedidos == null)
            {
                return HttpNotFound();
            }
            return View(pedidos);
        }

        // GET: Pedidos/Create
        public ActionResult Create()
        {
            ViewBag.Cliente = new SelectList(_db.Clientes, "Id_Cliente", "DNI_Cliente");
            return View();
        }

        // POST: Pedidos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id_Pedido,Cliente,Fecha_Pedido,Confirmado,Cobrado,Enviado,Recibido")] Pedidos pedidos)
        {
            if (ModelState.IsValid)
            {
                _db.Pedidos.Add(pedidos);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Cliente = new SelectList(_db.Clientes, "Id_Cliente", "DNI_Cliente", pedidos.Cliente);
            return View(pedidos);
        }

        // GET: Pedidos/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pedidos pedidos = await _db.Pedidos.FindAsync(id);
            if (pedidos == null)
            {
                return HttpNotFound();
            }
            ViewBag.Cliente = new SelectList(_db.Clientes, "Id_Cliente", "DNI_Cliente", pedidos.Cliente);
            return View(pedidos);
        }

        // POST: Pedidos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id_Pedido,Cliente,Fecha_Pedido,Confirmado,Cobrado,Enviado,Recibido")] Pedidos pedidos)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(pedidos).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.Cliente = new SelectList(_db.Clientes, "Id_Cliente", "DNI_Cliente", pedidos.Cliente);
            return View(pedidos);
        }

        // GET: Pedidos/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pedidos pedidos = await _db.Pedidos.FindAsync(id);
            if (pedidos == null)
            {
                return HttpNotFound();
            }
            return View(pedidos);
        }

        // POST: Pedidos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Pedidos pedidos = await _db.Pedidos.FindAsync(id);
            _db.Pedidos.Remove(pedidos);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
