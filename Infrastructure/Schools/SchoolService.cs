using Application.Features.Identity.Schools;
using Domain.Entities;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Schools
{
    public class SchoolService : ISchoolService
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public SchoolService(ApplicationDbContext applicationDbContext)
        {
            this._applicationDbContext=applicationDbContext;
        }
        public async Task<int> CreateAsync(School school)
        {
           await _applicationDbContext.Schools.AddAsync(school);
             await _applicationDbContext.SaveChangesAsync();
            return school.Id;
        }

        public async Task<int> DeleteAsync(School school)
        {
            _applicationDbContext.Schools.Remove(school);
            await _applicationDbContext.SaveChangesAsync();
            return school.Id;
        }

        public async Task<List<School>> GetAllAsync()
        => await _applicationDbContext.Schools.ToListAsync();

        public async Task<School> GetByIdAsync(int schoolId)
        {
            return await _applicationDbContext.Schools
                .Where(s=>s.Id == schoolId).FirstOrDefaultAsync();
        }

        public Task<List<School>> GetByNameAsync(string name)
        {
            return _applicationDbContext.Schools
                .Where(s => s.Name == name)
                .ToListAsync();
        }

        public async Task<int> UpdateAsync(School school)
        {
            _applicationDbContext.Schools.Update(school);
        await _applicationDbContext.SaveChangesAsync();
            return school.Id;
        }
    }
}
