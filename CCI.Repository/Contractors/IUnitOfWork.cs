using System.Data.Common;
using CCI.Repository.Contractors;

namespace CCI.Repository;

public interface IUnitOfWork
{
    DbConnection DbConnection();
    IUserRepository UserRepository { get; }
    IClientRoleRepository ClientRoleRepository { get; }
}
