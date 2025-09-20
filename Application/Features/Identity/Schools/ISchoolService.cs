using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Identity.Schools
{
    public interface ISchoolService
    {
        Task<int> CreateAsync(School school);
        Task<int> UpdateAsync(School school);
        Task<int> DeleteAsync(School school);
        Task<School> GetByIdAsync(int schoolId);
        Task<List<School>> GetAllAsync();
        Task<List<School>> GetByNameAsync(string name);
    }
}
