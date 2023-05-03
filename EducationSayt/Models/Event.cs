namespace EducationSayt.Models
{
    public class Event : BaseEntity
    {
        public string? Header { get; set; }
        public string? Location { get; set; }
        public DateTime Date { get; set; }
    }
}
