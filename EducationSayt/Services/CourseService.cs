using EducationSayt.Data;
using EducationSayt.Models;
using EducationSayt.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EducationSayt.Services
{
    public class CourseService : ICourseService
    {
        private readonly AppDbContext _context;
        public CourseService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Course>> GetAll() => await _context.Courses.Include(m => m.CourseImages).Where(m => !m.SoftDelete).ToListAsync();
        public async Task<Course> GetById(int id) => await _context.Courses.FindAsync(id);

        public async Task<IEnumerable<Author>> GetAuthorsAsync() =>  await _context.Authors.ToListAsync();
        public async Task<Course> GetFullDataById(int id) => await _context.Courses.Include(m => m.CourseImages).Include(m => m.Author).FirstOrDefaultAsync(m => m.Id == id);

        
    }
}
