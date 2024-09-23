using CCI.Domain.Contractors;
using Microsoft.AspNetCore.Identity;

namespace CCI.Domain.Entities
{
    public class Role : IdentityRole<Guid>, IEntity<Guid>
    {
        public string? Description { get; set; }
    }
}
