using System;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace DScrib2.Models
{
    public class Review
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }
        public string Slug { get; set; }
        public string AmazonID { get; set; }
        public int UserID { get; set; }

        [NotMapped]
        public bool Unsave { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }
    }
}