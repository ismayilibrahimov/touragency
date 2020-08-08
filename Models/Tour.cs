using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TourAgency.Models
{
    public class Tour
    {
        public int ID { get; set; }
        [Required]
        [StringLength(200)]
        public string Title { get; set; }
        [Required]
        [Range(0, 9999)]
        public int Price { get; set; }
        [Required]
        [StringLength(300)]
        public string ImageName { get; set; }
        [StringLength(1500)]
        public string Description { get; set; }
    }
}
