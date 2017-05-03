using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RentACar.Models
{
    public class CarEquipment : EntityBaseClass
    {
        public int VehicleID { get; set; }

        public int EquipmentID { get; set; }

        public double CombinationPrice { get; set; }

        public virtual Vehicle Vehicle { get; set; }

        public virtual Equipment Equipment { get; set; }
    }
}