using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using Microsoft.Owin.Security;
using Koala.Models;
using System.Collections.Generic;
using System.Net;

namespace Koala.Controllers
{
    [Authorize(Roles = "UserCliente")]
    public class CartController : BaseController
    {
        public async Task<ActionResult> Index()
        {
            var usuario = await base.GetUser();
            var cliente = await _db.Clientes.Where(c => c.DNI_Cliente == usuario.DNI)
                .FirstOrDefaultAsync();
            var listaCarrito = await _db.Carrito
                .Where(c => c.IdCliente == cliente.Id_Cliente).ToListAsync();
            var productos = await _db.Productos.ToListAsync();

            List<CartViewModel> lista = new List<CartViewModel>();

            foreach (var item in listaCarrito)
            {
                var producto = productos.First(p => p.Id_Producto == item.IdProducto);

                lista.Add(new CartViewModel
                {
                    Cantidad = item.Cantidad,
                    IdProducto = item.IdProducto,
                    NombreProducto = producto.Nombre,
                    Descuento = producto.Descuento,
                    Precio = producto.Precio,
                    //TotalPrecio = producto.Precio.Sum(p => p.Precio)
                });
            }
            return View(lista);
        }

        public async Task<ActionResult> AddProduct(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Productos producto = await _db.Productos.FindAsync(id);
            if (producto == null)
            {
                return HttpNotFound();
            }

            var usuario = await GetUser();
            var cliente = await _db.Clientes.Where(c => c.DNI_Cliente == usuario.DNI).FirstOrDefaultAsync();
            if (cliente == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var productoEnCarrito = await _db.Carrito.Where(c => c.IdProducto == id).FirstOrDefaultAsync();
            if (productoEnCarrito != null)
            {
                productoEnCarrito.Cantidad++;
                _db.Entry(productoEnCarrito).State = EntityState.Modified;
            }
            else
            {
                _db.Carrito.Add(new Carrito
                {
                    IdProducto = id.Value,
                    Cantidad = 1,
                    IdCliente = cliente.Id_Cliente
                });
            }
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
