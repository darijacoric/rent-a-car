using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RentACar.Models
{
    public enum EntityStatus : int { New = 0, Active = 1, Archived = -1 }
    
    public class EntityBaseClass
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID {get; set;}

        [Required]
        [ScaffoldColumn(false)]
        public EntityStatus Status { get; set; }

        [Required]
        [ScaffoldColumn(false)]
        public DateTime CreatedDate { get; set; }

        [Required]
        [ScaffoldColumn(false)]
        public DateTime ModifiedDate { get; set; }

        [NotMapped]
        [ScaffoldColumn(false)]
        public bool SetDates
        {
            get; set;
        }
    }
    
}