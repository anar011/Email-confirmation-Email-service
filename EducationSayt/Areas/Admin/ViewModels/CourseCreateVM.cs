using EducationSayt.Models;
using System.ComponentModel.DataAnnotations;

namespace EducationSayt.Areas.Admin.ViewModels
{
    public class CourseCreateVM
    {
        [Required]
        public string Price { get; set; }
        [Required]
        public string Header { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Sales { get; set; }

        public int AuthorId { get; set; }
        [Required]

        public List<IFormFile> Photos { get; set; }
    }



}
