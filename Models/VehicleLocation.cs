using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RentACar.Models
{
    public class VehicleLocation : EntityBaseClass
    {
        [Required]
        [StringLength(32)]
        [Display(Name = "Address")]
        public string Place { get; set; }

        [Required]
        [StringLength(32)]
        [Display(Name = "City")]
        public string City { get; set; }
        
        public byte[] LocationPicture { get; set; }

        [StringLength(512)]
        public string Description { get; set; }

        [NotMapped]
        [ScaffoldColumn(false)]
        public string DisplayLocation { get { return String.Format("{0}, {1}", Place, City ?? ""); }  }

        // For explanation: http://www.entityframeworktutorial.net/code-first/inverseproperty-dataannotations-attribute-in-code-first.aspx
        [InverseProperty("ReceiveLocation")]
        public virtual ICollection<Order> ReceiveOrders { get; set; }

        [InverseProperty("DestinationLocation")]
        public virtual ICollection<Order> DestinationOrders { get; set; }
    }
}