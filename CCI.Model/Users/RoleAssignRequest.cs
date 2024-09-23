using System.ComponentModel.DataAnnotations;

namespace CCI.Model;

public class RoleAssignRequest
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public string Role { get; set; }
}
