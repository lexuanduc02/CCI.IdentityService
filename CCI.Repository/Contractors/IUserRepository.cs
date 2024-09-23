using CCI.Domain.Entities;
using CCI.Model.Users;

namespace CCI.Repository.Contractors
{
    public interface IUserRepository : IRepository<User, Guid>
    {
        Task<UserViewModel> GetUserByIdAsync(Guid userId);
        Task<UserViewModel> GetUserByEmailAsync(string userEmail);
        Task<List<UserViewModel>> GetAllUserAsync();
        Task<string> GetRoleAsync(string userId);
    }
}
