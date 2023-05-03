using EducationSayt.Data;
using EducationSayt.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EducationSayt.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class NewController : Controller
    {

        private readonly AppDbContext _context;
        public NewController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<New> news = await _context.News.Where(m => !m.SoftDelete).ToListAsync();
            return View(news);
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();
            New news = await _context.News.FirstOrDefaultAsync(m => m.Id == id);
            if (news is null) return NotFound();
            return View(news);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
    }
}
