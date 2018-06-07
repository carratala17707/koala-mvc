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
                        NumArticulos = item.Línea_Pedido.Sum(p => p.Cantidad),
                        NumPedido = item.Id_Pedido,
                        TotalPrecio = item.Línea_Pedido.Sum(l => l.Precio * l.Cantidad),
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
                    string desc = string.Join(", ", item.Línea_Pedido.Select(l => l.Nombre));
                    order.Descripcion = desc;
                    orders.Add(order);
                }

                var storage = new Persistence.PhotoStorage();
                Uri uri = null;
                if (await storage.Exists(cliente.Foto))
                {
                    uri = await storage.GetBlobUri(cliente.Foto);
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
                        UsuarioID = cliente.Id_Cliente,
                        RutaFoto = uri != null ? uri.AbsoluteUri : null
                    },
                    Orders = orders
                };
                return View(model);
            }
            else if (User.IsInRole(KoalaRoles.UserAdmin))
            {
                var resultadoClientes = new List<ClientsViewModel>();
                var resultadoPedidos = new List<OrderViewModel>();
                var resultadoProductos = new List<ProductViewModel>();

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
                        Cliente = item.Clientes.Id_Cliente,
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
                    var product = new ProductViewModel
                    {
                        ID = item.Id_Producto,
                        Nombre = item.Nombre,
                        Descripcion = item.Descripcion,
                        Precio = item.Precio,
                        Tipo = item.Tipo,
                        NombreFoto = item.Foto,
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

        //EditUser de clientes
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditUser(ManageViewModel model)
        {
            if (ModelState.IsValid)
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
                    string nombreFoto = $"{System.Guid.NewGuid().ToString()}_{profile.NombreFoto}";
                    usuario.Foto = nombreFoto;

                    if (profile.FotoAttachment != null)
                    {
                        var storage = new Persistence.PhotoStorage();
                        await storage.UploadImage(profile.FotoAttachment.InputStream, nombreFoto);
                    }

                    _db.Entry(usuario).State = EntityState.Modified;
                    await _db.SaveChangesAsync();
                }
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        // GET: EditClient de Admin
        public async Task<ActionResult> EditClient(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Clientes cliente = await _db.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            var model = new ClientViewModel
            {
                Nombre = cliente.Nombre,
                Apellidos = cliente.Apellidos,
                Nick = cliente.Nick,
                DNI = cliente.DNI_Cliente,
                Direccion = cliente.Direccion,
                Email = cliente.Email,
                Telefono = cliente.Telefono,
                Poblacíon = cliente.Poblacion,
                FechaNacimiento = cliente.Fecha_Nacimiento,
                UsuarioID = cliente.Id_Cliente
            };
            model.Estados = GetEstadosCliente(cliente);
            return View(model);
        }

        private static List<SelectListItem> GetEstadosCliente(Clientes cliente)
        {
            List<SelectListItem> estados = new List<SelectListItem>();
            estados.Add(new SelectListItem
            {
                Value = EstadoCliente.Activo.ToString(),
                Text = EstadoCliente.Activo.ToString(),
                Selected = EstadoCliente.Activo.ToString() == cliente.Estado
            });
            estados.Add(new SelectListItem
            {
                Value = EstadoCliente.Amonestado.ToString(),
                Text = EstadoCliente.Amonestado.ToString(),
                Selected = EstadoCliente.Amonestado.ToString() == cliente.Estado
            });
            estados.Add(new SelectListItem
            {
                Value = EstadoCliente.Inactivo.ToString(),
                Text = EstadoCliente.Inactivo.ToString(),
                Selected = EstadoCliente.Inactivo.ToString() == cliente.Estado
            });
            return estados;
        }

        //POST:EditClient de Admin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditClient(ClientViewModel profile)
        {
            if (ModelState.IsValid)
            {
                var usuario = await _db.Clientes.Where(c => c.Id_Cliente == profile.UsuarioID)
                    .FirstOrDefaultAsync();
                if (usuario != null)
                {
                    usuario.Nombre = profile.Nombre;
                    usuario.Apellidos = profile.Apellidos;
                    usuario.DNI_Cliente = profile.DNI;
                    usuario.Direccion = profile.Direccion;
                    usuario.Email = profile.Email;
                    usuario.Fecha_Nacimiento = profile.FechaNacimiento;
                    usuario.Telefono = profile.Telefono;
                    usuario.Poblacion = profile.Poblacíon;
                    usuario.Estado = profile.DescripcionEstado;

                    _db.Entry(usuario).State = EntityState.Modified;
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            Clientes cliente = await _db.Clientes.FindAsync(profile.UsuarioID);
            profile.Estados = GetEstadosCliente(cliente);
            return View(profile);
        }

        //GET: EDITORDER
        // GET: EditClient de Admin
        public async Task<ActionResult> EditOrder(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pedidos pedido = await _db.Pedidos.FindAsync(id);
            if (pedido == null)
            {
                return HttpNotFound();
            }

            var model = new OrderViewModel
            {
                NumPedido = pedido.Id_Pedido,
                Cliente = pedido.Cliente,
                FechaPedido = pedido.Fecha_Pedido
            };
            if (pedido.Confirmado.HasValue)
            {
                model.FechaConfirmado = pedido.Confirmado.Value;
            }
            if (pedido.Recibido.HasValue)
            {
                model.FechaEnviado = pedido.Enviado.Value;
            }
            if (pedido.Recibido.HasValue)
            {
                model.FechaRecibido = pedido.Recibido.Value;
            }
            return View(model);
        }

        //POST EDITORDER
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditOrder(OrderViewModel order)
        {
            if (ModelState.IsValid)
            {
                var pedido = await _db.Pedidos.Where(p => p.Id_Pedido == order.NumPedido)
                    .FirstOrDefaultAsync();
                if (pedido != null)
                {
                    pedido.Fecha_Pedido = order.FechaPedido;
                    pedido.Confirmado = order.FechaConfirmado;
                    pedido.Enviado = order.FechaEnviado;
                    pedido.Recibido = order.FechaRecibido;

                    _db.Entry(pedido).State = EntityState.Modified;
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            Pedidos pedidos = await _db.Pedidos.FindAsync(order.NumPedido);
            return View(order);
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
        public async Task<ActionResult> CreateProduct(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.FotoAttachment != null)
                {
                    string nombreFoto = $"{System.Guid.NewGuid().ToString()}_{model.NombreFoto}";
                    var storage = new Persistence.PhotoStorage();
                    await storage.UploadImage(model.FotoAttachment.InputStream, nombreFoto);
                }

                _db.Productos.Add(new Productos
                {
                    Id_Producto = model.ID,
                    Nombre = model.Nombre,
                    Descripcion = model.Descripcion,
                    Precio = model.Precio,
                    Tipo = model.Tipo,
                    Descuento = model.Descuento,
                    Foto = model.NombreFoto,
                    Escaparate = model.Escaparate
                });

                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Tipo = new SelectList(_db.Tipo_Producto, "Id_Tipo", "Descripcion", model.Tipo);
            return View(model);
        }

        // GET: EditProduct
        public async Task<ActionResult> EditProduct(int? id)
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

            var storage = new Persistence.PhotoStorage();
            Uri uri = null;
            if (await storage.Exists(producto.Foto))
            {
                uri = await storage.GetBlobUri(producto.Foto);
            }

            var model = new ProductViewModel
            {
                ID = producto.Id_Producto,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Precio = producto.Precio,
                Tipo = producto.Tipo,
                NombreFoto = producto.Foto,
                RutaFoto = uri.AbsoluteUri,
                Descuento = producto.Descuento,
                Escaparate = producto.Escaparate
            };
            ViewBag.Tipo = new SelectList(_db.Tipo_Producto, "Id_Tipo", "Descripcion", producto.Tipo);
            return View(model);
        }

        // POST: EditProduct
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditProduct(ProductViewModel product)
        {
            if (ModelState.IsValid)
            {
                var producto = await _db.Productos.Where(p => p.Id_Producto == product.ID)
                    .FirstOrDefaultAsync();

                if (product != null)
                {
                    producto.Id_Producto = product.ID;
                    producto.Nombre = product.Nombre;
                    producto.Descripcion = product.Descripcion;
                    producto.Precio = product.Precio;
                    producto.Tipo = product.Tipo;
                    producto.Descuento = product.Descuento;
                    producto.Escaparate = product.Escaparate;
                    string nombreFoto = $"{System.Guid.NewGuid().ToString()}_{product.NombreFoto}";
                    producto.Foto = nombreFoto;

                    if (product.FotoAttachment != null)
                    {
                        var storage = new Persistence.PhotoStorage();
                        await storage.UploadImage(product.FotoAttachment.InputStream, nombreFoto);
                    }

                    _db.Entry(producto).State = EntityState.Modified;
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            ViewBag.Tipo = new SelectList(_db.Tipo_Producto, "Id_Tipo", "Descripcion", product.Tipo);
            return View(product);
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
        [HttpPost, ActionName("DeleteProduct")]
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