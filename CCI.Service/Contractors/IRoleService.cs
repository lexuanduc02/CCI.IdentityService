using CCI.Model;
using CCI.Model.CommonModels;

namespace CCI.Service;

public interface IRoleService
{
    Task<BaseResponseModel<bool>> RoleAssign(RoleAssignRequest request);
}
