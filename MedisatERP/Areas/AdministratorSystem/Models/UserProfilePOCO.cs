/* 
 * UserProfilePOCO serves as a Plain Old CLR Object (POCO) to facilitate
 * the organization and transfer of user-related data in the business logic layer.
 * This class is NOT an Entity Framework-managed entity and does not interact directly
 * with the database or require inclusion in a DbContext.
 * It helps map incoming user data to relevant models (e.g., AspNetUser)
 * for further processing or database operations.
 */

public class UserProfilePOCO
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
    public Guid? CompanyId { get; set; }
    public string BioData { get; set; }
    public string Gender { get; set; }
    public string Country { get; set; }
    public string Name { get; set; }
    public DateTime? Dob { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
