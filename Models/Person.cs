using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RentACar.Models
{
    // Nepotrebno
    public class Person : EntityBaseClass
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

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d/M/yyyy}")]
        public DateTime BirthDate { get; set; }

        public byte[] ProfilePicture { get; set; }
                
        public string Email { get; set; }
    }
}