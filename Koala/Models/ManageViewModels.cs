using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace Koala.Models
{
    public class ManageViewModel
    {
        public ProfileViewModel Profile { get; set; }
        public List<OrderViewModel> Orders { get; set; }
        //public bool HasPassword { get; set; }
        //public IList<UserLoginInfo> Logins { get; set; }
        //public string PhoneNumber { get; set; }
        //public bool TwoFactor { get; set; }
        //public bool BrowserRemembered { get; set; }
    }

    public class OrderViewModel
    {
        public int NumPedido { get; set; }
        public System.DateTime FechaPedido { get; set; }
        public System.DateTime FechaConfirmado { get; set; }
        public System.DateTime FechaPagado { get; set; }
        public System.DateTime FechaEnviado { get; set; }
        public System.DateTime FechaRecibido { get; set; }
        public double totalPrecio { get; set; }
    }


    public class ProfileViewModel
    {
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Nick { get; set; }
        public string Contraseña { get; set; }
        public string DNI { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string Poblacíon { get; set; }
        public System.DateTime FechaNacimiento { get; set; }
        public string Email { get; set; }
        public string Foto { get; set; }
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