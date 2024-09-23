using CCI.Domain.EF;
using CCI.Domain.Entities;
using CCI.Model.Users;
using CCI.Repository.Contractors;
using Dapper;

namespace CCI.Repository
{
    public class UserRepository : Repository<User, Guid>, IUserRepository
    {
        public const string RoleAdmin = "c17d487e-646a-47e2-9ee4-b319155e326e";
        public UserRepository(DataContext context) : base(context)
        {
        }

        public async Task<List<UserViewModel>> GetAllUserAsync()
        {
            var queryString =
                $"""
                    SELECT "Users".*, "Roles"."NormalizedName" as "Role" 
                    FROM "Users"
                    LEFT JOIN "UserRoles" ON "Users"."Id" = "UserRoles"."UserId"
                    LEFT JOIN "Roles" On "Roles"."Id" = "UserRoles"."RoleId"
                    WHERE "RoleId" != '{RoleAdmin}' OR "RoleId" IS NULL 
                """;

            var result = await Context.GetDbConnect().QueryAsync<UserViewModel>(queryString);

            return result.ToList();
        }

        public async Task<string> GetRoleAsync(string userId)
        {
            var queryString =
                $"""
                    SELECT "Roles"."Name" as "Role" 
                    FROM "Users"
                    LEFT JOIN "UserRoles" ON "Users"."Id" = "UserRoles"."UserId"
                    LEFT JOIN "Roles" On "Roles"."Id" = "UserRoles"."RoleId"
                    WHERE "Users"."Id" = '{userId}'
                """;

            var result = await Context.GetDbConnect().QueryAsync<string>(queryString);

            return result.FirstOrDefault();
        }

        public async Task<UserViewModel> GetUserByEmailAsync(string userEmail)
        {
            var queryString =
                $"""
                    SELECT "Users".*, "Roles"."NormalizedName" as "Role" 
                    FROM "Users"
                    LEFT JOIN "UserRoles" ON "Users"."Id" = "UserRoles"."UserId"
                    LEFT JOIN "Roles" On "Roles"."Id" = "UserRoles"."RoleId"
                    WHERE "Email" = {userEmail}
                """;

            var result = await Context.GetDbConnect().QueryAsync<UserViewModel>(queryString);

            return (UserViewModel)result;
        }

        public async Task<UserViewModel> GetUserByIdAsync(Guid userId)
        {
            var queryString =
                $"""
                    SELECT "Users".*, "Roles"."NormalizedName" as "Role" 
                    FROM "Users"
                    LEFT JOIN "UserRoles" ON "Users"."Id" = "UserRoles"."UserId"
                    LEFT JOIN "Roles" On "Roles"."Id" = "UserRoles"."RoleId"
                    WHERE "Id" = {userId}
                """;

            var result = await Context.GetDbConnect().QueryAsync<UserViewModel>(queryString);

            return (UserViewModel)result;
        }
    }
}
