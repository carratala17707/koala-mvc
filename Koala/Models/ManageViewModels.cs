using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Koala.Helpers;
using System.Web;

namespace Koala.Models
{
    public class ManageViewModel
    {
        public ProfileViewModel Profile { get; set; }
        public List<OrderViewModel> Orders { get; set; }
        public List<ClientsViewModel> Clients { get; set; }
        public List<ProductViewModel> Products { get; set; }
        //public bool HasPassword { get; set; }
        //public IList<UserLoginInfo> Logins { get; set; }
        //public string PhoneNumber { get; set; }
        //public bool TwoFactor { get; set; }
        //public bool BrowserRemembered { get; set; }
    }

    public class ProductViewModel
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public string Tipo { get; set; }
        public string Foto { get; set; }
        [Display(Name = "Foto")]
        [HttpPostedFileExtensions(Extensions = "jpg,jpeg,png,bmp", ErrorMessage = "Formato de fichero inválido.")]
        public HttpPostedFileBase FotoAttachment { get; set; }
        public double Descuento { get; set; }
        public bool Escaparate { get; set; }
    }

    public class ClientsViewModel
    {
        public int ID { get; set; }
        public string DNI { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Estado { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string Direccion { get; set; }
        public string Poblacion { get; set; }
        public string NickCliente { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public System.DateTime FechaNac { get; set; }
    }

    public class OrderViewModel
    {
        public int NumPedido { get; set; }
        public int Cliente { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public System.DateTime FechaPedido { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public System.DateTime FechaConfirmado { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public System.DateTime FechaPagado { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public System.DateTime FechaEnviado { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public System.DateTime FechaRecibido { get; set; }
        public decimal TotalPrecio { get; set; }
        public string Descripcion { get; set; }
        public int NumArticulos { get; set; }
        public EstadoPedido Estado { get; set; }

        public enum EstadoPedido
        {
            Confirmado, Pagado, Enviado, Recibido
        }
    }

    public class ProfileViewModel
    {
        [Required]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }
        [Required]
        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; }
        [Required]
        [Display(Name = "Nick")]
        public string Nick { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "La {0} debe ser al menos de {2} carácteres de longitud.", MinimumLength = 3)]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva contraseña")]
        public string Contraseña { get; set; }
        [Required]
        [Display(Name = "DNI")]
        public string DNI { get; set; }
        [Required]
        [Display(Name = "Teléfono")]
        [DataType(DataType.PhoneNumber)]
        public string Telefono { get; set; }
        [Required]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; }
        [Required]
        [Display(Name = "Estado")]
        public EstadoCliente Estado { get; set; }
        [Required]
        [Display(Name = "Población")]
        public string Poblacíon { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Fecha de nacimiento")]
        public System.DateTime FechaNacimiento { get; set; }
        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string NombreFoto { get; set; }
        [Display(Name = "Foto")]
        [HttpPostedFileExtensions(Extensions = "jpg,jpeg,png,bmp", ErrorMessage = "Formato de fichero inválido.")]
        public HttpPostedFileBase FotoAttachment { get; set; }
        public int UsuarioID { get; set; }
    }

    public enum EstadoCliente
    {
        Activo, Inactivo, Amonestado
    }

    public class ClientViewModel
    {
        [Required]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }
        [Required]
        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; }
        [Required]
        [Display(Name = "Nick")]
        public string Nick { get; set; }
        [Required]
        [Display(Name = "DNI")]
        public string DNI { get; set; }
        [Required]
        [Display(Name = "Teléfono")]
        [DataType(DataType.PhoneNumber)]
        public string Telefono { get; set; }
        [Required]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; }
        [Required]
        [Display(Name = "Estado")]
        public EstadoCliente Estado { get; set; }
        [Required]
        [Display(Name = "Población")]
        public string Poblacíon { get; set; }
        [Required]
        [Display(Name = "Fecha de nacimiento")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public System.DateTime FechaNacimiento { get; set; }
        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public int UsuarioID { get; set; }

        public IEnumerable<System.Web.Mvc.SelectListItem> Estados { get; set; }
        public string DescripcionEstado { get; set; }

        public enum EstadoCliente
        {
            Activo, Inactivo, Amonestado
        }
    }

    public class ManageLoginsViewModel
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }
        public IList<AuthenticationDescription> OtherLogins { get; set; }
    }

    public class FactorViewModel
    {
        public string Purpose { get; set; }
    }

    public class SetPasswordViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class AddPhoneNumberViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string Number { get; set; }
    }

    public class VerifyPhoneNumberViewModel
    {
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }

    public class ConfigureTwoFactorViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
    }
}