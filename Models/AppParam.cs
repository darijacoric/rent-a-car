using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RentACar.Models
{
    public class AppParam
    {
        [Key]
        public string ParamCode { get; set; }

        public string ParamDescription { get; set; }

        public EntityStatus Status { get; set; }

        public virtual ICollection<AppParamValues> AppParamValues { get; set; }

        
    }
}