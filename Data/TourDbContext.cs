using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourAgency.Models;

namespace TourAgency.Data
{
    public class TourDbContext : IdentityDbContext
    {
        public TourDbContext(DbContextOptions<TourDbContext> options) : base(options)
        {

        }

        public DbSet<Tour> Tours { get; set; }
    }
}
