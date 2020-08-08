using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using TourAgency.Data;
using TourAgency.Models;

namespace TourAgency.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly TourDbContext _context;
        private readonly IWebHostEnvironment _hostEnv;

        public AdminController(TourDbContext context, IWebHostEnvironment hostEnv)
        {
            _context = context;
            _hostEnv = hostEnv;
        }

        [Route("admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Tours.ToListAsync());
        }

        // GET: Admin/Details/5
        [Route("admin/details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tour = await _context.Tours
                .FirstOrDefaultAsync(m => m.ID == id);
            if (tour == null)
            {
                return NotFound();
            }

            return View(tour);
        }

        // GET: Admin/Create
        [HttpGet("admin/create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("admin/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TourViewModel model)
        {
            Tour tour = null;
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                if(model.Image.Length > 0 && model.Image.Length < 5000000)
                {
                    if (CheckExtention(model.Image.FileName))
                    {
                        string uploadFolder = Path.Combine(_hostEnv.WebRootPath, "images");
                        uniqueFileName = Path.GetRandomFileName() + "_" + model.Image.FileName;
                        var filePath = Path.Combine(uploadFolder, uniqueFileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.Image.CopyToAsync(stream);
                        }
                    }
                    else
                    {
                        ViewBag.Error = "Extention is invalid, select jpg or png file!";
                        return View();
                    }
                }
                else
                {
                    ViewBag.Error = "Image size is too big, please select 5mb or less size image!";
                    return View();
                }

                tour = new Tour
                {
                    Title = model.Title,
                    Description = model.Description,
                    Price = model.Price,
                    ImageName = uniqueFileName
                };

                _context.Add(tour);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tour);
        }

        // GET: Admin/Edit/5
        [HttpGet("admin/edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tour = await _context.Tours.FindAsync(id);
            if (tour == null)
            {
                return NotFound();
            }
            return View(tour);
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("admin/edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, 
            [Bind("ID,Title,Price,ImageName,Description")] Tour tour, IFormFile fileObject)
        {

            if (id != tour.ID)
            {
                return NotFound();
            }


            string uniqueFileName = null;
            if (!(fileObject == null))
            {
                string uploadFolder = Path.Combine(_hostEnv.WebRootPath, "images");
                
                if (fileObject.Length > 0 && fileObject.Length < 5000000)
                {
                    if (CheckExtention(fileObject.FileName))
                    {
                        uniqueFileName = Path.GetRandomFileName() + "_" + fileObject.FileName;
                        var filePath = Path.Combine(uploadFolder, uniqueFileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await fileObject.CopyToAsync(stream);
                        }
                        System.IO.File.Delete(Path.Combine(uploadFolder, tour.ImageName));
                        tour.ImageName = uniqueFileName;
                    }
                    else
                    {
                        ViewBag.Error = "Extention is invalid, select jpg or png file!";
                        return View();
                    }
                }
                else
                {
                    ViewBag.Error = "Image size is too big, please select 5mb or less size image!";
                    return View();
                }

            }


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tour);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TourExists(tour.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tour);
        }

        // GET: Admin/Delete/5
        [HttpGet("admin/delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tour = await _context.Tours
                .FirstOrDefaultAsync(m => m.ID == id);
            if (tour == null)
            {
                return NotFound();
            }

            return View(tour);
        }

        // POST: Admin/Delete/5
        [HttpPost("admin/delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string uploadFolder = Path.Combine(_hostEnv.WebRootPath, "images");

            var tour = await _context.Tours.FindAsync(id);

            System.IO.File.Delete(Path.Combine(uploadFolder, tour.ImageName));

            _context.Tours.Remove(tour);
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TourExists(int id)
        {
            return _context.Tours.Any(e => e.ID == id);
        }

        private bool CheckExtention(string uploadedFileName)
        {
            string[] permittedExtensions = { ".jpg", ".jpeg", ".png" };

            var ext = Path.GetExtension(uploadedFileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            {
                return false;
            }

            return true;
        }


}
}
