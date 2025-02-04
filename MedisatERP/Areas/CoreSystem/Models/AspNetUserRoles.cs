using MedisatERP.Areas.CoreSystem.Models;
using System.ComponentModel.DataAnnotations;


namespace MedisatERP.Areas.CoreSystem.Models;

public partial class AspNetUserRoles
{
    [Key]
    public string UserId { get; set; }
    public string RoleId { get; set; }

    public virtual AspNetUser User { get; set; }
    public virtual AspNetRole Role { get; set; }
}
