using CCI.Model.CommonModels;
using CloudinaryDotNet.Actions;

namespace CCI.Service.Contractors;

public interface IPhotoService
{
    Task<ImageUploadResult> AddPhotoAsync(UploadImageModel model);
    void DeletePhotoAsync(String publicId);
}
