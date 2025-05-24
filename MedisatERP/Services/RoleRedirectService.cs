namespace MedisatERP.Services
{

    public interface IRoleRedirectService
    {
        string GenerateAdminToCompRedirectUrl(IList<string> roles);
        string GenerateRedirectUrl(IList<string> roles);
    }

    public class RoleRedirectService : IRoleRedirectService
    {
        public string GenerateRedirectUrl(IList<string> roles)
        {
            if (roles.Contains("Sys_System Manager"))
            {
                return "/AdministratorSystem/Dashboards/Crm";
            }
            else if (roles.Contains("Comp_System Administrator"))
            {
                return "/NutritionCompanySystem/Dashboards/Crm";
            }
            return null;
        }

        public string GenerateAdminToCompRedirectUrl(IList<string> roles)
        {
            if (roles.Contains("Sys_System Manager") && roles.Contains("Comp_System Administrator"))
            {
                return "/NutritionCompanySystem/Dashboards/Security";
            }
            return null;
        }
    }
}
