using MedisatERP.Areas.AdministratorSystem.Models;
using MedisatERP.Areas.NutritionCompanySystem.Models;
using MedisatERP.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace MedisatERP.Data
{
    public class SharedDbContext : DbContext
    {
        public SharedDbContext(DbContextOptions<SharedDbContext> options) : base(options) { }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            

        }
    }
}
