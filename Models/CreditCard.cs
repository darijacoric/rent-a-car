using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RentACar.Models
{
    public class CreditCard : EntityBaseClass
    {
        [Required]
        [RegularExpression("^[0-9]{16}$", ErrorMessage = "Card number must contain only numbers.")]
        [DataType(DataType.CreditCard)]
        [StringLength(16, MinimumLength = 16, ErrorMessage = "Card number must consist of 16 numbers.")]
        [Display(Name = "Credit Card Number")]
        public string CardNumber { get; set; }

        [Required]
        [Display(Name = "Expiration Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d/M/yyyy}")]
        public DateTime ExpirationDate { get; set; }

        [Required]
        [StringLength(30)]
        [RegularExpression(@"^[A-ZČĆŽŠĐ]+[a-zA-ZČĆŽŠĐčćžšđ''-'\s]*$")]
        [Display(Name = "Owner's First Name")]
        public string OwnerFirstName { get; set; }

        [Required]
        [StringLength(30)]
        [RegularExpression(@"^[A-ZČĆŽŠĐ]+[a-zA-ZČĆŽŠĐčćžšđ''-'\s]*$")]
        [Display(Name = "Owner's Last Name")]
        public string OwnerLastName { get; set; }

        [ScaffoldColumn(false)]
        public int UserId { get; set; }

        [Required]
        [Display(Name = "Payment Type")]
        public int PaymentTypeId { get; set; }

        public virtual PaymentType PaymentType { get; set; }

        [ForeignKey("UserId")]
        public virtual AppUser User { get; set; }
    }
}