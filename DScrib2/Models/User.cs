using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DScrib2.Models
{
    public class User
    {
        public int ID { get; set; }
        public string Email { get; set; }
        public string VendorID { get; set; }

        public virtual List<Review> Reviews { get; set; }
    }
}