using Microsoft.AspNetCore.Mvc;
using MedisatERP.Data;  // Assuming this namespace contains the DbContext
using Microsoft.EntityFrameworkCore;
using MedisatERP.Library; // Assuming this has any helper methods for decoding or processing IDs, if needed

namespace MedisatERP.Areas.NutritionCompany.Controllers
{
    [Area("NutritionCompany")]
    [Route("NutritionCompany/[controller]/[action]/{companyId?}")]
    public class PatientsController : Controller
    {
        private readonly MedisatErpDbContext _dbContext;

        // Constructor to inject the DbContext
        public PatientsController(MedisatErpDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: /NutritionCompany/Patients/Index/{patientId}
        public async Task<IActionResult> Index(string companyId)
        {
            if (string.IsNullOrEmpty(companyId))
            {
                return BadRequest("Company ID is required.");
            }

            try
            {
                // Decode the companyId from the URL
                var decodedCompanyId = HashingHelper.DecodeGuidID(companyId);

                // Retrieve the company using decodedCompanyId from the database
                var company = await _dbContext.Companies
                    .Where(c => c.CompanyId == decodedCompanyId)
                    .FirstOrDefaultAsync();


                if (company == null)
                {
                    return NotFound();  // Return a 404 if the company is not found
                }

                // Optionally, pass company data to the view
                return View(company);  // Pass the company to the view
            }
            catch (FormatException)
            {
                // Handle invalid Base64 string
                return BadRequest("Invalid company ID format.");
            }
        }
    }
}
