using RentACar.App_Helpers;
using RentACar.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RentACar.ViewModels
{
    public class VehicleViewModel
    {
        public VehicleViewModel()
        {
            Vehicle = new Vehicle();
            AdditionalEquipment = new List<AssignedEquipment>();
            Files = new List<HttpPostedFileBase>();
            ImagesBase64 = new List<AppValues.ImageViewModel>();
        }

        public Vehicle Vehicle { get; set; }

        public List<AssignedEquipment> AdditionalEquipment { get; set; }

        public struct AssignedEquipment
        {
            public Equipment Equipment { get; set; }

            public bool Assigned { get; set; }
        }


        [ScaffoldColumn(false)]
        //[DataType(DataType.Upload)]
        public List<HttpPostedFileBase> Files { get; set; }

        [NotMapped]
        [ScaffoldColumn(false)]
        public List<AppValues.ImageViewModel> ImagesBase64 { get; set; }
    }
    
}