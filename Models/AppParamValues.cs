using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RentACar.Models
{
    public class AppParamValues
    {
        [Key]
        public int ID { get; set; }

        public string ParamCode { get; set; }

        public string ParamValue { get; set; }

        public EntityStatus Status { get; set; }
        
        [ForeignKey("ParamCode")]                
        public virtual AppParam AppParams { get; set; }

    }
}