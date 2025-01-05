using MedisatERP.Areas.CoreSystem.Models;


namespace MedisatERP.Areas.CoreSystem.Models;

public partial class AspNetUserRoles
{
    public string UserId { get; set; }
    public string RoleId { get; set; }

    public virtual AspNetUser User { get; set; }
    public virtual AspNetRole Role { get; set; }
}
