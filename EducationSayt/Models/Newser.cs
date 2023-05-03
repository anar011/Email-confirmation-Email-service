namespace EducationSayt.Models
{
    public class Newser : BaseEntity
    {
        public string? Fullname { get; set; }
        public ICollection<New> News { get; set; }
    }
}
