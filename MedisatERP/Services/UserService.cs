using MedisatERP.Data;
using MedisatERP.Models;
using Microsoft.EntityFrameworkCore;

namespace MedisatERP.Services
{

    public class UserService
    {
        private readonly ApplicationDbContext _dbContext;

        public UserService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<AspNetUser> GetUserAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User ID is required.");
            }

            // Retrieve the user using the userId from the db
            var user = await _dbContext.AspNetUsers
                                       .Where(c => c.Id == userId)
                                       .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new KeyNotFoundException("User not found."); 
            }

            return user; // Return the user object
        }

        public async Task<Company> GetCompanyAsync(Guid companyId)
        {
            if (companyId == Guid.Empty)
            {
                throw new ArgumentException("User ID is required.");
            }

            var company = await _dbContext.Companies.Where(c => c.CompanyId == companyId).FirstOrDefaultAsync();

            if (company == null)
            {
                throw new KeyNotFoundException("User not found."); 
            }

            return company;
        }
    }


}
