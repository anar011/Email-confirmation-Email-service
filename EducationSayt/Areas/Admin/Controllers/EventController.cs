using EducationSayt.Data;
using EducationSayt.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EducationSayt.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EventController : Controller
    {

        private readonly AppDbContext _context;
        public EventController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Event> events = await _context.Events.Where(m => !m.SoftDelete).ToListAsync();
            return View(events);
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();
            Event events = await _context.Events.FirstOrDefaultAsync(m => m.Id == id);
            if (events is null) return NotFound();
            return View(events);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
    }
}
