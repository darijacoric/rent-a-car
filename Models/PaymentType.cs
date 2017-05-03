using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RentACar.Models
{
    public class PaymentType : EntityBaseClass
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public virtual ICollection<CreditCard> CreditCards { get; set; }
    }
}