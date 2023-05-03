using EducationSayt.Models;

namespace EducationSayt.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Slider> Slider { get; set; }
        public IEnumerable<Course> Courses { get; set; }
        public IEnumerable<Event> Events { get; set; }
        public IEnumerable<Newser> Newsers { get; set; }
    }
}
