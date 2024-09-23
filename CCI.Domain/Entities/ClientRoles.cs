using System.ComponentModel.DataAnnotations.Schema;
using CCI.Domain.Contractors;

namespace CCI.Domain;

[Table("ClientRoles")]
public class ClientRoles : IEntity<int>
{
    public string RoleName { get; set; }
    public string ClientId { get; set; }
    public int Id { get; set; }
}
