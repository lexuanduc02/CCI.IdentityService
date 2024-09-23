using CCI.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CCI.Domain.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(new Role
            {
                Id = new Guid("C17D487E-646A-47E2-9EE4-B319155E326E"),
                Name = "admin",
                NormalizedName = "ADMIN"
            });

            modelBuilder.Entity<Role>().HasData(new Role
            {
                Id = new Guid("E029D545-0A04-4088-B156-0F1AFA8EF68B"),
                Name = "recruiter",
                NormalizedName = "RECRUITER"
            });

            modelBuilder.Entity<Role>().HasData(new Role
            {
                Id = new Guid("3C51F349-A63E-43B1-B31F-6A7A0ADDF485"),
                Name = "user",
                NormalizedName = "USER"
            });

            var hasher = new PasswordHasher<User>();
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = new Guid("D7D6AE65-8029-46C5-A006-F89D6D04FA8C"),
                UserName = "admin",
                Email = "lexuanduc.hn@gmail.com",
                PasswordHash = hasher.HashPassword(null, "lxduc11@111"),
                FirstName = "Admin",
                LastName = "",
                PhoneNumber = "0987654321",
            });

            modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(new IdentityUserRole<Guid>
            {
                RoleId = new Guid("C17D487E-646A-47E2-9EE4-B319155E326E"),
                UserId = new Guid("D7D6AE65-8029-46C5-A006-F89D6D04FA8C")
            });
        }

    }
}
