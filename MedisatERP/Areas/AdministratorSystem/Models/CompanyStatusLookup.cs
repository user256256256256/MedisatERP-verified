using System.ComponentModel.DataAnnotations;

namespace MedisatERP.Areas.AdministratorSystem.Models
{
    public class CompanyStatusLookup
    {
        [Key]
        public int StatusId { get; set; } 
        public string StatusName { get; set; }

        public virtual ICollection<Company> Companies { get; set; } = new List<Company>();
    }
}
