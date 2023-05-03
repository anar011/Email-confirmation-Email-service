namespace EducationSayt.Models
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public bool SoftDelete { get; set; } = false;
        public DateTime Date { get; set; }
    }
}
