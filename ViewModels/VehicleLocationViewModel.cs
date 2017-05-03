using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using RentACar.Models;
using System.ComponentModel.DataAnnotations.Schema;
using RentACar.App_Helpers;

namespace RentACar.ViewModels
{
    public class VehicleLocationViewModel
    {
        public VehicleLocationViewModel()
        {
            VehicleLocation = new VehicleLocation();
            Files = new List<HttpPostedFileBase>();
            ImagesBase64 = new List<AppValues.ImageViewModel>();
        }
        
        public VehicleLocation VehicleLocation { get; set; }

        [ScaffoldColumn(false)]
        //[DataType(DataType.Upload)]
        public List<HttpPostedFileBase> Files { get; set; }

        [NotMapped]
        [ScaffoldColumn(false)]
        public List<AppValues.ImageViewModel> ImagesBase64 { get; set; }
    }
}