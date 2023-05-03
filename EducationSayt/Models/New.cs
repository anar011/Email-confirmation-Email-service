namespace EducationSayt.Models
{
    public class New : BaseEntity
    {
        public string? Image { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public int NewserId { get; set; }
        public Newser Newser { get; set; }
    }
}
