using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TourAgency.Models
{
    public class BookingViewModel
    {
        [Required]
        [StringLength(200)]
        public string FirstName { get; set; }
        [StringLength(200)]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        [StringLength(300)]
        public string Email { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        [Range(0, 999999999999999999)]
        public long PhoneNumber { get; set; }
        [Range(1, 999)]
        public int Person { get; set; }
        [Required]
        [StringLength(9)]
        [RegularExpression(@"^[0-9]*$")]
        public string Price { get; set; }
        [Required]
        [StringLength(300)]
        public string TourName { get; set; }
        [StringLength(1500)]
        public string Note { get; set; }
    }
}
