using EducationSayt.Areas.Admin.ViewModels;
using EducationSayt.Data;
using EducationSayt.Helpers;
using EducationSayt.Models;
using EducationSayt.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace EducationSayt.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CourseController : Controller
    {

        private readonly AppDbContext _context;
        private readonly ICourseService _courseService;
        private readonly IWebHostEnvironment _env;
        public CourseController(AppDbContext context,
                                ICourseService courseService,
                                IWebHostEnvironment env)
        {
            _context = context;
            _courseService = courseService;
            _env = env;
        }


        public async Task<IActionResult> Index()
        {
            IEnumerable<Course> courses = await _context.Courses.Include(c=>c.CourseImages).Include(c=>c.Author).Where(m => !m.SoftDelete).ToListAsync();
            return View(courses);
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();
            Course course = await _context.Courses.Include(c=>c.CourseImages).Include(c=>c.Author).FirstOrDefaultAsync(m => m.Id == id);
            if (course is null) return NotFound();
            return View(course);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.author = await GetAuthorAsync();
            return View();
        }

        private async Task<SelectList> GetAuthorAsync()
        {
            IEnumerable<Author> authors = await _courseService.GetAuthorsAsync();
            return new SelectList(authors, "Id", "FullName");
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseCreateVM model)
        {
            try
            {
                ViewBag.author = await GetAuthorAsync();

                if (!ModelState.IsValid)
                {
                    return View();
                }

                foreach (var photo in model.Photos)
                {
                    if (!photo.CheckFileType("image/"))
                    {
                        ModelState.AddModelError("Photo", "File type must be image");
                        return View();
                    }

                    if (!photo.CheckFileSize(200))
                    {
                        ModelState.AddModelError("Photo", "Image size must be max 200kb");
                        return View();
                    }
                }

                List<CourseImage> courseImages = new();

                foreach (var photo in model.Photos)
                {
                    string fileName = Guid.NewGuid().ToString() + "_" + photo.FileName;

                    string path = FileHelper.GetFilePath(_env.WebRootPath, "images", fileName);

                    await FileHelper.SaveFileAsync(path, photo);

                    CourseImage courseImage = new()
                    {
                        Image = fileName
                    };

                    courseImages.Add(courseImage);
                }

                courseImages.FirstOrDefault().IsMain = true;
                decimal convertedPrice = decimal.Parse(model.Price.Replace(".", ","));

                Course newCourse = new()
                {
                    Price = convertedPrice,
                    Header = model.Header,
                    Description = model.Description,
                    Sales = model.Sales,
                    AuthorId = model.AuthorId,
                    CourseImages = courseImages
                };
                await _context.CourseImages.AddRangeAsync(courseImages);
                await _context.Courses.AddAsync(newCourse);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {

                throw;
            }
        }



        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();
            Course course = await _courseService.GetFullDataById((int)id);
            if (course == null) return NotFound();
            ViewBag.desc = Regex.Replace(course.Description, "<.*?>", String.Empty);
            return View(course);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteProduct(int? id)
        {
        if (id == null) return BadRequest();
        Course course = await _courseService.GetFullDataById((int)id);
        if (course == null) return NotFound();

            foreach (var item in course.CourseImages)
            {
                string path = FileHelper.GetFilePath(_env.WebRootPath, "images", item.Image);
                FileHelper.DeleteFile(path);
            }
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public async Task<IActionResult> DeleteProductImage(int? id)
        {
            if (id == null) return BadRequest();

            bool result = false;

            CourseImage courseImage = await _context.CourseImages.Where(m => m.Id == id).FirstOrDefaultAsync();

            if (courseImage == null) return NotFound();

            var data = await _context.Courses.Include(m => m.CourseImages).FirstOrDefaultAsync(m => m.Id == courseImage.CourseId);

            if (data.CourseImages.Count > 1)
            {
                string path = FileHelper.GetFilePath(_env.WebRootPath, "images", courseImage.Image);

                FileHelper.DeleteFile(path);

                _context.CourseImages.Remove(courseImage);

                await _context.SaveChangesAsync();

                result = true;
            }

            data.CourseImages.FirstOrDefault().IsMain = true;

            await _context.SaveChangesAsync();

            return Ok(result);

        }



        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return BadRequest();

            ViewBag.categories = await GetAuthorAsync();

            Course dbCroduct = await _courseService.GetFullDataById((int)id);

            if (dbCroduct == null) return NotFound();


            CourseEditVM model = new()
            {
                Id = dbCroduct.Id,
                Sales = dbCroduct.Sales,
                Header = dbCroduct.Header,
                Price = dbCroduct.Price.ToString("0.#####"),
                AuthorId = dbCroduct.AuthorId,
                CourseImages = dbCroduct.CourseImages.ToList(),
                Description = dbCroduct.Description
            };


            return View(model);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, CourseEditVM updatedCourse)
        {
            if (id == null) return BadRequest();

            ViewBag.categories = await GetAuthorAsync();

            Course dbCourse = await _context.Courses.AsNoTracking().Include(m => m.CourseImages).Include(m => m.Author).FirstOrDefaultAsync(m => m.Id == id);

            if (dbCourse == null) return NotFound();

            if (!ModelState.IsValid)
            {
                updatedCourse.CourseImages = dbCourse.CourseImages.ToList();
                return View(updatedCourse);
            }

            List<CourseImage> courseImages = new();

            if (updatedCourse.Photos is not null)
            {
                foreach (var photo in updatedCourse.Photos)
                {
                    if (!photo.CheckFileType("image/"))
                    {
                        ModelState.AddModelError("Photo", "File type must be image");
                        updatedCourse.CourseImages = dbCourse.CourseImages.ToList();
                        return View(updatedCourse);
                    }

                    if (!photo.CheckFileSize(200))
                    {
                        ModelState.AddModelError("Photo", "Image size must be max 200kb");
                        updatedCourse.CourseImages = dbCourse.CourseImages.ToList();
                        return View(updatedCourse);
                    }
                }



                foreach (var photo in updatedCourse.Photos)
                {
                    string fileName = Guid.NewGuid().ToString() + "_" + photo.FileName;

                    string path = FileHelper.GetFilePath(_env.WebRootPath, "images", fileName);

                    await FileHelper.SaveFileAsync(path, photo);

                    CourseImage courseImage = new()
                    {
                        Image = fileName
                    };

                    courseImages.Add(courseImage);
                }

                await _context.CourseImages.AddRangeAsync(courseImages);
            }

            decimal convertedPrice = decimal.Parse(updatedCourse.Price.Replace(".", ","));

            Course newCourse = new()
            {
                Id = dbCourse.Id,
                Header = updatedCourse.Header,
                Price = convertedPrice,
                Description = updatedCourse.Description,
                AuthorId = updatedCourse.AuthorId,
                Sales = updatedCourse.Sales,
                CourseImages = courseImages.Count == 0 ? dbCourse.CourseImages : courseImages
            };


            _context.Courses.Update(newCourse);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


    }
}
