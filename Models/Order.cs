using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RentACar.Models
{
    public class Order : EntityBaseClass
    {
        [Required]
        [Display(Name = "Order Number")]
        public int OrderNumber { get; set; }
               
        [Required]
        [Display(Name = "Pickup Location")]
        public int RecvLocationId { get; set; }

        [Required]
        [Display(Name = "Pickup Time")]
        public DateTime RecvTime { get; set; }
               
        [Required]
        [Display(Name = "Destination")]
        public int DestLocationId { get; set; }

        [Required]
        [Display(Name = "Destination Time")]
        public DateTime DestTime { get; set; }

        [Required]
        public int VehicleId { get; set; }

        [Required]
        public int UserId { get; set; } // Cannot set navigation property otherwise Order clas will be in UserDbContext

        [Required]
        [Display(Name = "Total Price")]
        [DisplayFormat(DataFormatString = "{0:N2} kn")]
        public double TotalPrice { get; set; }

        [Required]
        public int PaymentTypeId { get; set; }
        
        public int CreditCardId { get; set; }

        [ForeignKey("RecvLocationId")]
        public virtual VehicleLocation ReceiveLocation { get; set; }

        [ForeignKey("DestLocationId")]
        public virtual VehicleLocation DestinationLocation { get; set; }

        [ForeignKey("VehicleId")]
        public virtual Vehicle Vehicle { get; set; }                
    }
}