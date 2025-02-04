using MedisatERP.Areas.AdministratorSystem.Models;
using System.ComponentModel.DataAnnotations;


namespace MedisatERP.Areas.AdministratorSystem.Models;

public partial class AspNetUserRoles
{
    [Key]
    public string UserId { get; set; }
    public string RoleId { get; set; }

    public virtual AspNetUser User { get; set; }
    public virtual AspNetRole Role { get; set; }
}
