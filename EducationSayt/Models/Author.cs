namespace EducationSayt.Models
{
    public class Author : BaseEntity
    {
        public string Image { get; set; }
        public string FullName { get; set; }
        public ICollection<Course> Courses { get; set; }
    }
}
