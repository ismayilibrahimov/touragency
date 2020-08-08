using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TourAgency.Models
{
    public class TourViewModel
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; }
        [Required]
        [Range(0, 9999)]
        public int Price { get; set; }
        [Required]
        public IFormFile Image { get; set; }
        [Required]
        [StringLength(1500)]
        public string Description { get; set; }
    }
}
