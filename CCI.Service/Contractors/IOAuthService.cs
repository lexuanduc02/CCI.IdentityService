using CCI.Domain.Entities;
using CCI.Model;
using CCI.Model.CommonModels;
using CCI.Model.OAuthModels;
using CCI.Model.ResponseModel;

namespace CCI.Service.Contractors
{
    public interface IOAuthService
    {
        Task<BaseResponseModel<string>> RecoveryPassword(string email);
        Task<BaseResponseModel<string>> ResetPassword(ResetPasswordModel request);
        Task<BaseResponseModel<User>> Register(RegisterModel request);
        Task<BaseResponseModel<string>> ChangePassword(ChangePasswordModel request);
        Task<BaseResponseModel<LoginResponseModel>> Login(LoginRequest request);
        Task<BaseResponseModel<LoginResponseModel>> ReLogin(ReLoginRequest request);
        Task<BaseResponseModel<string>> Logout(LogoutRequest request);
    }
}
