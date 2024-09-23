using CCI.Domain;
using CCI.Domain.EF;
using Dapper;

namespace CCI.Repository;

public class ClientRoleRepository : Repository<ClientRoles, int>, IClientRoleRepository
{
    public ClientRoleRepository(DataContext context) : base(context)
    {

    }
}
