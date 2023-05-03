
using EducationSayt.Data;
using EducationSayt.Models;
using EducationSayt.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EducationSayt.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Slider> sliders = await _context.Sliders.Where(m => !m.SoftDelete).ToListAsync();
            IEnumerable<Course> courses = await _context.Courses.Include(m => m.Author).Include(m=>m.CourseImages).Where(m => !m.SoftDelete).ToListAsync();
            IEnumerable<Event> events = await _context.Events.Where(m => !m.SoftDelete).ToListAsync();
            IEnumerable<Newser> newsers = await _context.Newsers.Include(m => m.News).Where(m => !m.SoftDelete).ToListAsync();
            HomeVM model = new()
            {
                Slider = sliders,
                Courses = courses,
                Events = events,
                Newsers = newsers
            };
            return View(model);
        }


    }
}