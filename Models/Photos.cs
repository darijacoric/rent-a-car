using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RentACar.Models
{
    public class Photos : EntityBaseClass
    {
        public int ItemID { get; set; }

        public string ItemType { get; set; }

        public string PictureName { get; set; }

        public byte[] Content { get; set; }

        public string PictureType { get; set; }

        public EntityStatus? ProfilePicture { get; set; }        

    }
}