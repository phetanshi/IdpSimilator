using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ps.IdentityServer.Data;

[Table("UserGroups")]
public class UserGroup
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string GroupName { get; set; } = null!;
    public string? UserId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
}
