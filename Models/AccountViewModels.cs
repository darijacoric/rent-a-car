using RentACar.App_Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace RentACar.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [StringLength(30)]
        [RegularExpression(@"^[A-ZČĆŽŠĐ]+[a-zA-ZČĆŽŠĐčćžšđ''-'\s]*$")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [RegularExpression(@"^[A-ZČĆŽŠĐ]+[a-zA-ZČĆŽŠĐčćžšđ''-'\s]*$")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d/M/yyyy}")]
        public DateTime? BirthDate { get; set; }

        public byte[] ProfilePicture { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [StringLength(30)]
        [RegularExpression(@"^[A-ZČĆŽŠĐ]+[a-zA-ZČĆŽŠĐčćžšđ''-'\s]*$")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [RegularExpression(@"^[A-ZČĆŽŠĐ]+[a-zA-ZČĆŽŠĐčćžšđ''-'\s]*$")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d/M/yyyy}")]
        public DateTime? BirthDate { get; set; }
        
        [Display(Name = "Postal Code")]
        [DataType(DataType.PostalCode)]
        public string PostalCode { get; set; }

        [StringLength(30)]
        [Display(Name = "City")]
        public string City { get; set; }

        [StringLength(40)]
        [Display(Name = "Street Name")]
        public string StreetName { get; set; }
        
        [Display(Name = "Phone Number")]
        [StringLength(40)]
        [RegularExpression(@"^[0-9]*", ErrorMessage = "Input only numbers for Phone Number.")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public int Role { get; set; }

        [ScaffoldColumn(false)]
        //[DataType(DataType.Upload)]
        public List<HttpPostedFileBase> Files { get; set; }
    }

    public class EditViewModel
    {
        public EditViewModel()
        {
            Files = new List<HttpPostedFileBase>();
            ImagesBase64 = new List<AppValues.ImageViewModel>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(30)]
        [RegularExpression(@"^[A-ZČĆŽŠĐ]+[a-zA-ZČĆŽŠĐčćžšđ''-'\s]*$")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [RegularExpression(@"^[A-ZČĆŽŠĐ]+[a-zA-ZČĆŽŠĐčćžšđ''-'\s]*$")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d/M/yyyy}")]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Postal Code")]
        [DataType(DataType.PostalCode)]
        public string PostalCode { get; set; }

        [StringLength(30)]
        [Display(Name = "City")]
        public string City { get; set; }

        [StringLength(40)]
        [Display(Name = "Street Name")]
        public string StreetName { get; set; }
        
        //[RegularExpression(@"^[0-9]*", ErrorMessage = "Input only numbers for Phone Number.")]        
        [Phone()]
        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(40)]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        
        public int Role { get; set; }

        [ScaffoldColumn(false)]
        //[DataType(DataType.Upload)]
        public List<HttpPostedFileBase> Files { get; set; }

        [ScaffoldColumn(false)]
        public List<AppValues.ImageViewModel> ImagesBase64 { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
