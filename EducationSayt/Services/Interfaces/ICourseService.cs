using EducationSayt.Models;

namespace EducationSayt.Services.Interfaces
{
    public interface ICourseService
    {
        Task<Course> GetById(int id);
        Task<IEnumerable<Course>> GetAll();
        Task<Course> GetFullDataById(int id);
        Task<IEnumerable<Author>> GetAuthorsAsync();
    }
}
