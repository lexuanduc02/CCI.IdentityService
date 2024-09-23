using CCI.Model;
using CCI.Model.CommonModels;
using CCI.Model.ResponseModel;

namespace CCI.Service;

public interface ITokenService
{
    Task<BaseResponseModel<LoginResponseModel>> RefreshAccessToken(RefreshAccessTokenRequest request);
    Task<BaseResponseModel<string>> IntrospectToken(IntrospectTokenRequest request);
}
