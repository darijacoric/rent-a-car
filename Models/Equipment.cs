using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace RentACar.Models
{
    public class Equipment : EntityBaseClass
    {
        [Required]
        [StringLength(32)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Price")]
        [DisplayFormat(DataFormatString = "{0:N2} kn")]
        public double Price { get; set; }
        
        [StringLength(512)]        
        [Display(Name = "Description")]
        public string Description { get; set; }

        public virtual ICollection<CarEquipment> CarEquipment { get; set; }
    }
}