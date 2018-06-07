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
                if (item != null)
                {
                    double precio = (double)(item.Cantidad * producto.Precio);
                    double descuento = (100 - producto.Descuento) / 100;
                    double precioConDescuento = precio * descuento;
                    lista.Add(new CartViewModel
                    {
                        IdCarrto = item.IdCarrito,
                        Cantidad = item.Cantidad,
                        IdProducto = item.IdProducto,
                        NombreProducto = producto.Nombre,
                        Descuento = producto.Descuento,
                        Precio = precioConDescuento
                    });
                }
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

        [HttpPost]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Carrito carrito = await _db.Carrito.FindAsync(id);
            if (carrito == null)
            {
                return HttpNotFound();
            }
            _db.Carrito.Remove(carrito);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> MakeOrder()
        {
            var usuario = await GetUser();
            var cliente = await _db.Clientes.Where(c => c.DNI_Cliente == usuario.DNI).FirstOrDefaultAsync();
            var productos = await _db.Productos.ToListAsync();
            if (cliente != null)
            {
                var articulosCarrito = await _db.Carrito.Where(c => c.IdCliente == cliente.Id_Cliente).ToListAsync();

                var pedido = new Pedidos
                {
                    Cliente = cliente.Id_Cliente,
                    Fecha_Pedido = DateTime.Now,
                    Confirmado = DateTime.Now
                };
                _db.Pedidos.Add(pedido);
                await _db.SaveChangesAsync();

                foreach (var item in articulosCarrito)
                {
                    var producto = productos.Find(p => p.Id_Producto == item.IdProducto);
                    _db.Línea_Pedido.Add(new Línea_Pedido
                    {
                        Pedido = pedido.Id_Pedido,
                        Cantidad = item.Cantidad,
                        Precio = producto.Precio,
                        Nombre = producto.Nombre,
                        Producto = producto.Id_Producto
                    });
                }
                await _db.SaveChangesAsync();

                _db.Carrito.RemoveRange(articulosCarrito);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return View("Index");
        }
    }
}
