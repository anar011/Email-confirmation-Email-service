using EducationSayt.Models;
using System.ComponentModel.DataAnnotations;

namespace EducationSayt.Areas.Admin.ViewModels
{
    public class CourseEditVM
    {
        public int Id { get; set; }
        [Required]
        public string Price { get; set; }
        [Required]
        public string Header { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Sales { get; set; }
        public int AuthorId { get; set; }
        public ICollection<CourseImage> CourseImages { get; set; }
        public List<IFormFile> Photos { get; set; }
    }
}
