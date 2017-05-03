using RentACar.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RentACar.ViewModels
{
    // Step 1
    public class DateAndLocationViewModel
    {
        [Required]
        [Display(Name = "City")]
        public string ReceiveCity { get; set; }

        [Required]
        [Display(Name = "Address")]
        public string ReceivePlace { get; set; }

        [Required]
        [Display(Name = "Pickup Time")]
        [DataType(DataType.DateTime)]
        public DateTime ReceiveTime { get; set; }

        [Required]
        [Display(Name = "City")]
        public string DestinationCity { get; set; }

        [Required]
        [Display(Name = "Address")]
        public string DestinationPlace { get; set; }

        [Required]
        [Display(Name = "Drop Off Time")]
        [DataType(DataType.DateTime)]
        public DateTime DestinationTime { get; set; }
    }

    // Step 2
    public class VehicleSelectionViewModel
    {
        public VehicleSelectionViewModel()
        {
            AdditionalEquipment = new List<Equipment>();
        }

        public int ID { get; set; }

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
                
        [Display(Name = "Number of Seats")]
        public int? SeatCount { get; set; }
        
        [Display(Name = "Fuel Type")]
        public string Fuel { get; set; }
        
        [Display(Name = "Mileage (km)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double? Mileage { get; set; }

        [Display(Name = "Number of Airbags")]
        public int Airbags { get; set; }

        [Display(Name = "Model Year")]
        public string ModelYear { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        [DataType(DataType.Currency)]
        [Display(Name = "Price for one Day")]
        public double DayPrice { get; set; }    
                
        public double TotalPriceForPeriod { get; set; }

        public string ImagesBase64 { get; set; }

        public List<Equipment> AdditionalEquipment { get; set; }                
    }

    // Step 3
    public class PaymentTypeViewModel
    {
        [Required]
        [Display(Name = "Payment Type")]
        public int PaymentTypeId { get; set; }

        [Required]        
        [DataType(DataType.CreditCard)]
        [StringLength(16, MinimumLength = 16, ErrorMessage = "Card number must consist of 16 numbers.")]
        [RegularExpression("^[0-9]{16}$", ErrorMessage = "Card number must contain only numbers.")]
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
    }

    // Step 4
    public class OrderSummaryViewModel
    {
        public Order Order { get; set; }

        public AppUser User { get; set; }

        public VehicleSelectionViewModel VehicleModel { get; set; }

        public string ReceiveLocationImageBase64 { get; set; }

        public string DestinationLocationImageBase64 { get; set; }

        public string Html { get; set; }
    }

    public class OrderViewModel
    {
        public Order Order { get; set; }

        public AppUser User { get; set; }

        public CreditCard CreditCard { get; set; }

        public PaymentType PaymentType { get; set; }
    }
}