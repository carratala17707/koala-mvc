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
    [Authorize]
    public class ManageController : BaseController
    {
        private ApplicationSignInManager _signInManager;

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        // GET: /Manage/Index
        public async Task<ActionResult> Index()
        {
            if (User.IsInRole(KoalaRoles.UserCliente))
            {
                var orders = new List<OrderViewModel>();

                var pedidos = await _db.Pedidos.Include(p => p.Clientes)
                    .Include(l => l.Línea_Pedido).ToListAsync();
                var usuario = await base.GetUser();
                var cliente = await _db.Clientes.Where(c => c.DNI_Cliente == usuario.DNI)
                    .FirstOrDefaultAsync();
                pedidos = pedidos.FindAll(p => p.Cliente == cliente.Id_Cliente);

                foreach (var item in pedidos)
                {
                    var order = new OrderViewModel
                    {
                        Descripcion = "",
                        NumArticulos = item.Línea_Pedido.Count(),
                        NumPedido = item.Id_Pedido,
                        TotalPrecio = item.Línea_Pedido.Sum(l => l.Precio),
                        FechaPedido = item.Fecha_Pedido,
                    };
                    if (item.Confirmado.HasValue)
                    {
                        order.FechaConfirmado = item.Confirmado.Value;
                        order.Estado = OrderViewModel.EstadoPedido.Confirmado;
                    }
                    if (item.Cobrado.HasValue)
                    {
                        order.FechaPagado = item.Cobrado.Value;
                        order.Estado = OrderViewModel.EstadoPedido.Pagado;
                    }
                    if (item.Enviado.HasValue)
                    {
                        order.FechaEnviado = item.Enviado.Value;
                        order.Estado = OrderViewModel.EstadoPedido.Enviado;
                    }
                    if (item.Recibido.HasValue)
                    {
                        order.FechaRecibido = item.Recibido.Value;
                        order.Estado = OrderViewModel.EstadoPedido.Recibido;
                    }
                    string desc = string.Empty;
                    foreach (var linea in item.Línea_Pedido)
                    {
                        if (linea.Productos != null)
                        {
                            desc += linea.Productos.Nombre + ", ";
                        }
                    }
                    order.Descripcion = desc;
                    orders.Add(order);
                }

                var model = new ManageViewModel
                {
                    Profile = new ProfileViewModel
                    {
                        Nombre = cliente.Nombre,
                        Apellidos = cliente.Apellidos,
                        Nick = cliente.Nick,
                        Contraseña = usuario.Contraseña,
                        DNI = cliente.DNI_Cliente,
                        Email = cliente.Email,
                        FechaNacimiento = cliente.Fecha_Nacimiento,
                        Direccion = cliente.Direccion,
                        Poblacíon = cliente.Poblacion,
                        Telefono = cliente.Telefono,
                        NombreFoto = cliente.Foto,
                        UsuarioID = cliente.Id_Cliente
                    },
                    Orders = orders
                };
                return View(model);
            }
            else if (User.IsInRole(KoalaRoles.UserAdmin))
            {
                var resultadoClientes = new List<ClientsViewModel>();
                var resultadoPedidos = new List<OrderViewModel>();
                var resultadoProductos = new List<ProductsViewModel>();

                var clientes = await _db.Clientes.ToListAsync();
                var pedidos = await _db.Pedidos.Include(p => p.Clientes)
                     .Include(l => l.Línea_Pedido).ToListAsync();
                var productos = await _db.Productos.ToListAsync();
                var usuario = await base.GetUser();
                var administrador = await _db.Administradores.Where(a => a.DNI_Admin == usuario.DNI)
                    .FirstOrDefaultAsync();

                foreach (var item in clientes)
                {
                    var client = new ClientsViewModel
                    {
                        ID = item.Id_Cliente,
                        Nombre = item.Nombre,
                        Apellidos = item.Apellidos,
                        NickCliente = item.Nick,
                        DNI = item.DNI_Cliente,
                        Telefono = item.Telefono,
                        Email = item.Email,
                        Direccion = item.Direccion,
                        Poblacion = item.Poblacion,
                        FechaNac = item.Fecha_Nacimiento,
                        Estado = item.Estado
                    };
                    resultadoClientes.Add(client);
                }

                foreach (var item in pedidos)
                {
                    var order = new OrderViewModel
                    {
                        NumPedido = item.Id_Pedido,
                        Cliente = item.Clientes.Nick,
                        Descripcion = "",
                        TotalPrecio = item.Línea_Pedido.Sum(l => l.Precio),
                        FechaPedido = item.Fecha_Pedido,
                    };
                    if (item.Confirmado.HasValue)
                    {
                        order.FechaConfirmado = item.Confirmado.Value;
                        order.Estado = OrderViewModel.EstadoPedido.Confirmado;
                    }
                    if (item.Cobrado.HasValue)
                    {
                        order.FechaPagado = item.Cobrado.Value;
                        order.Estado = OrderViewModel.EstadoPedido.Pagado;
                    }
                    if (item.Enviado.HasValue)
                    {
                        order.FechaEnviado = item.Enviado.Value;
                        order.Estado = OrderViewModel.EstadoPedido.Enviado;
                    }
                    if (item.Recibido.HasValue)
                    {
                        order.FechaRecibido = item.Recibido.Value;
                        order.Estado = OrderViewModel.EstadoPedido.Recibido;
                    }
                    string desc = string.Empty;
                    foreach (var linea in item.Línea_Pedido)
                    {
                        if (linea.Productos != null)
                        {
                            desc += linea.Productos.Nombre + ", ";
                        }
                    }
                    order.Descripcion = desc;
                    resultadoPedidos.Add(order);
                }

                foreach (var item in productos)
                {
                    var product = new ProductsViewModel
                    {
                        ID = item.Id_Producto,
                        Nombre = item.Nombre,
                        Descripcion = item.Descripcion,
                        Precio = item.Precio,
                        Tipo = item.Tipo,
                        Foto = item.Foto,
                        Descuento = item.Descuento,
                        Escaparate = item.Escaparate
                    };
                    resultadoProductos.Add(product);
                }

                var model = new ManageViewModel
                {
                    Orders = resultadoPedidos,
                    Clients = resultadoClientes,
                    Products = resultadoProductos
                };

                return View("Admin", model);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditUser(ManageViewModel model)
        {
            var profile = model.Profile;
            var usuario = await _db.Clientes.Where(c => c.Id_Cliente == profile.UsuarioID)
                .FirstOrDefaultAsync();
            if (usuario != null)
            {
                usuario.Nombre = profile.Nombre;
                usuario.Apellidos = profile.Apellidos;
                usuario.Direccion = profile.Direccion;
                usuario.Email = profile.Email;
                usuario.Fecha_Nacimiento = profile.FechaNacimiento;
                usuario.Telefono = profile.Telefono;
                usuario.Poblacion = profile.Poblacíon;

                _db.Entry(usuario).State = EntityState.Modified;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: Productos/Create
        public ActionResult CreateProduct()
        {
            ViewBag.Tipo = new SelectList(_db.Tipo_Producto, "Id_Tipo", "Descripcion");
            return View();
        }

        // POST: Productos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateProduct([Bind(Include = "Id_Producto,Nombre,Descripcion,Precio,Tipo,Foto,Descuento,Escaparate")] Productos productos)
        {
            if (ModelState.IsValid)
            {
                _db.Productos.Add(productos);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Tipo = new SelectList(_db.Tipo_Producto, "Id_Tipo", "Descripcion", productos.Tipo);
            return View(productos);
        }

        // GET: Productos/Edit/5
        public async Task<ActionResult> EditProduct(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Productos productos = await _db.Productos.FindAsync(id);
            if (productos == null)
            {
                return HttpNotFound();
            }
            ViewBag.Tipo = new SelectList(_db.Tipo_Producto, "Id_Tipo", "Descripcion", productos.Tipo);
            return View(productos);
        }

        // POST: Productos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditProduct([Bind(Include = "Id_Producto,Nombre,Descripcion,Precio,Tipo,Foto,Descuento,Escaparate")] Productos productos)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(productos).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.Tipo = new SelectList(_db.Tipo_Producto, "Id_Tipo", "Descripcion", productos.Tipo);
            return View(productos);
        }

        // GET: Productos/Delete/5
        public async Task<ActionResult> DeleteProduct(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Productos productos = await _db.Productos.FindAsync(id);
            if (productos == null)
            {
                return HttpNotFound();
            }
            return View(productos);
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Productos productos = await _db.Productos.FindAsync(id);
            _db.Productos.Remove(productos);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: /Manage/OrderDetails/id
        public async Task<ActionResult> OrderDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var pedido = await _db.Pedidos.Include(l => l.Línea_Pedido)
                .Where(p => p.Id_Pedido == id).FirstOrDefaultAsync();
            if (pedido == null)
            {
                return HttpNotFound();
            }
            return View(pedido);
        }

        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("ManageLogins", new { Message = message });
        }

        //
        // GET: /Manage/AddPhoneNumber
        public ActionResult AddPhoneNumber()
        {
            return View();
        }

        //
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Generate the token and send it
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
            if (UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = "Your security code is: " + code
                };
                await UserManager.SmsService.SendAsync(message);
            }
            return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        }

        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // GET: /Manage/VerifyPhoneNumber
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
            // Send an SMS through the SMS provider to verify the phone number
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        //
        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "Failed to verify phone");
            return View(model);
        }

        //
        // POST: /Manage/RemovePhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemovePhoneNumber()
        {
            var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Manage/ManageLogins
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        //
        // GET: /Manage/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        #endregion
    }
}