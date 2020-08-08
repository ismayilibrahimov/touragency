using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MimeKit;
using TourAgency.Data;
using TourAgency.Models;

namespace TourAgency.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TourDbContext _context;

        public HomeController(TourDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }


        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("tours")]
        public async Task<IActionResult> Tour()
        {
            return View(await _context.Tours.ToListAsync());
        }

        [HttpGet("booking/{id}")]
        public async Task<IActionResult> Booking(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tour = await _context.Tours
                .FirstOrDefaultAsync(m => m.ID == id);
            var titles = await _context.Tours.Select(u => u.Title).ToListAsync();
            ViewBag.Titles = titles;
            ViewBag.Tour = tour;

            if (tour == null)
            {
                return NotFound();
            }

            return View();
        }


        [HttpPost("booking/{id}")]
        public IActionResult Mail(BookingViewModel booking)
        {
            if(ModelState.IsValid)
            {
                ViewBag.Info = "mail has sent successfully!";
                _ = SendMail(booking);
                return View();
            }

            ViewBag.Info = "mail can not sent";
            return View();
        }

        private async Task SendMail(BookingViewModel booking)
        {

            var mailBody = booking.FirstName + " " +
                           booking.LastName + " | " +
                           booking.Email + " | " +
                           booking.Date + " | " +
                           booking.PhoneNumber + " | " +
                           booking.TourName + " | " +
                           booking.Person + " | $" +
                           booking.Price + " | " +
                           booking.Note;

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("customer", "your@mail.com"));
            message.To.Add(new MailboxAddress("admin", "your@mail.com"));
            message.Subject = "subject section";
            message.Body = new TextPart("plain") { Text = mailBody };


            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, false);
                await client.AuthenticateAsync("your@mail.com", "password");
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }

        }


        [Route("about")]
        public IActionResult About()
        {
            return View();
        }

        [Route("contact")]
        public IActionResult Contact()
        {
            return View();
        }

        [Route("terms")]
        public IActionResult Terms()
        {
            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
