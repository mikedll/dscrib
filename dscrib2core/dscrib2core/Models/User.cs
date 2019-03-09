using System.Collections.Generic;

namespace DScrib2.Models
{
    public class User
    {
        public int ID { get; set; }
        public string Email { get; set; }
        public string VendorID { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }
    }
}