using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RentACar.Models
{
    public enum Available { Free = 1, Taken = -1 }
    public enum FuelType { Diesel = 0, Petrol = 1, Gas = 2 }

    public class Vehicle : EntityBaseClass
    {
        // todo: Add property how many gas it holds
        public Vehicle()
        {
            Photos = new List<Photos>();
        }

        [Required]
        [Display(Name = "Vehicle Type")]
        public string VehicleType { get; set; }

        [Required]
        [Display(Name = "Brand Name")]
        public string Brand { get; set; }

        [Required]
        [Display(Name = "Brand Type")]
        public string BrandType { get; set; }
        
        [Required]
        [Display(Name = "Gas Per 100 km [l/100 km]")]
        [DisplayFormat(DataFormatString = "{0:N1}")]
        [Range(0, 1000)]
        public double GasPerKm { get; set; }

        [Range(0, 999999)]
        [Display(Name = "Horsepower (HP)")]
        public int? HorsePower { get; set; }

        [Range(0, 8)]
        [Display(Name = "Number of Doors")]
        public int? DoorsCount { get; set; }

        [Required]
        [Display(Name = "Registration Plate")]
        [RegularExpression(@"^[A-Z][A-Z]\-[0-9]*\-[A-Z][A-Z]", ErrorMessage = "Registration number is not valid. Example: RI-123-TZ")]        
        public string Registration { get; set; }

        [Required]
        [Display(Name = "Registration Expire Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d/M/yyyy}")]
        public DateTime? RegistrationExpire { get; set; }

        [ScaffoldColumn(false)]
        public Available? Free { get; set; }

        [Range(0, 12)]
        [Display(Name = "Number of Seats")]
        public int? SeatCount { get; set; }
                                        
        [ForeignKey("Fuel")]
        [Display(Name = "Fuel Type")]
        public int? FuelId { get; set; }

        [Range(0, 99999999.99)]
        [Display(Name = "Mileage (km)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double? Mileage { get; set; }

        [Required]
        //[DisplayFormat(DataFormatString = "{0:C2}")]
        [DisplayFormat(DataFormatString = "{0:N2} kn")]
        [Display(Name = "Price for one Day")]
        public double DayPrice { get; set; }

        [Required]
        [Display(Name = "Acquired Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d/M/yyyy}")]
        public DateTime AcquiredDate { get; set; }

        [Required]
        [Range(0, 12)]
        [Display(Name = "Number of Airbags")]
        public int Airbags { get; set; }
        
        [Required]
        [Display(Name = "Model Year")]
        public string ModelYear { get; set; }

        public byte[] Picture { get; set; }

        [NotMapped]
        [ScaffoldColumn(false)]
        public List<Photos> Photos { get; set; }

        public virtual ICollection<CarEquipment> CarEquipment { get; set; }
        
        public virtual Fuel Fuel { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}