using CCI.Common.Enums;
using CCI.Domain.Contractors;
using Microsoft.AspNetCore.Identity;

namespace CCI.Domain.Entities
{
    public class User : IdentityUser<Guid>, IEntity<Guid>
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public bool IsActive { get; set; }

        public string? UserCode { get; set; }

        public string? Title { get; set; }

        public string? ImageLink { get; set; }

        public int Gender { get; set; }

        public DateTime Dob { get; set; }
    }
}
