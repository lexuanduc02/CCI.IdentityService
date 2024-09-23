using System.Data.Common;
using CCI.Domain.EF;
using CCI.Domain.Entities;
using CCI.Repository.Contractors;

namespace CCI.Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly DataContext _dataContext;

    public UnitOfWork(DataContext dataContext)
    {
        _dataContext = dataContext;
        UserRepository = new UserRepository(_dataContext);
        ClientRoleRepository = new ClientRoleRepository(_dataContext);
    }
    public DbConnection DbConnection()
    {
        return _dataContext.GetDbConnect();
    }

    #region Register Repositories
    public IUserRepository UserRepository { get; }
    public IClientRoleRepository ClientRoleRepository { get; }
    #endregion

}
