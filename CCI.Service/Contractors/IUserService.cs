using CCI.Model;
using CCI.Model.CommonModels;
using CCI.Model.Users;

namespace CCI.Service.Contractors
{
    public interface IUserService
    {
        Task<BaseResponseModel<List<UserViewModel>>> GetAllUser();
        Task<BaseResponseModel<UserViewModel>> GetUser(Guid id);
        Task<BaseResponseModel<string>> UpdateUser(UpdateUserRequest request);
        Task<BaseResponseModel<bool>> LockUser(Guid userId);
        Task<BaseResponseModel<bool>> UnLockUser(Guid userId);
        Task<BaseResponseModel<string>> UploadImage(UploadAvatarModel model);
        Task<BaseResponseModel<bool>> RemoveImage(String publicId);
    }
}
