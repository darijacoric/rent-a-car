using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RentACar.Models
{
    public class Fuel : EntityBaseClass
    {
        public string FuelName { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2} kn")]
        public double? PricePerLiter { get; set; }
    }
}