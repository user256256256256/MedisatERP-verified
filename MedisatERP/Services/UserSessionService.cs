using MedisatERP.Data;
using MedisatERP.Models;
using Microsoft.EntityFrameworkCore;

namespace MedisatERP.Services
{

    public interface IUserSessionService
    {
        Task<AspNetUser> GetUserSessionAsync(string email);
        void SetSessionData(string userId, Guid? companyId);
    }

    public class UserSessionService : IUserSessionService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserSessionService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AspNetUser> GetUserSessionAsync(string email)
        {
            var user = await _dbContext.Set<AspNetUser>()
                .Where(u => u.Email == email)
                .Select(u => new AspNetUser { Id = u.Id, CompanyId = u.CompanyId })
                .FirstOrDefaultAsync();

            if (user?.CompanyId.HasValue == true)
            {
                var company = await _dbContext.Companies.Where(c => c.CompanyId == user.CompanyId).Select(c => new { c.CompanyLogoFilePath }).FirstOrDefaultAsync();

                if (company != null)
                {
                    var session = _httpContextAccessor.HttpContext.Session;
                    session.SetString("CompanyLogoFilePath", company.CompanyLogoFilePath);
                }
            }

            return user;
        }

        public void SetSessionData(string userId, Guid? companyId)
        {
            var session = _httpContextAccessor.HttpContext.Session;
            session.SetString("UserId", userId);

            if (companyId.HasValue)
            {
                session.SetString("CompanyId", companyId.Value.ToString());
            }
        }
    }

}
