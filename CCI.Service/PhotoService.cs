using CCI.Common.Extensions;
using CCI.Model;
using CCI.Model.CommonModels;
using CCI.Service.Contractors;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog.Events;

namespace CCI.Service
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;
        private readonly ILogger<PhotoService> _logger;
        private const string ClassName = nameof(EmailService);

        public PhotoService(ILogger<PhotoService> logger,
        IOptions<CloudinaryOption> options)
        {
            _logger = logger;

            var acc = new Account()
            {
                Cloud = options.Value.CloudName,
                ApiKey = options.Value.ApiKey,
                ApiSecret = options.Value.ApiSecret,
            };

            _cloudinary = new Cloudinary(acc);
        }

        public async Task<ImageUploadResult> AddPhotoAsync(UploadImageModel model)
        {
            var uploadResult = new ImageUploadResult();

            try
            {
                if (model.File.Length > 0)
                {
                    using var stream = model.File.OpenReadStream();
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(model.FileName, stream),
                        Transformation = new Transformation().Height(model.Height).Width(model.With).Crop("fill").Gravity("face"),
                        DisplayName = model.FileName,
                        PublicId = model.PublicId,
                        Folder = model.Folder,
                    };

                    uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    _logger.LogInformation("Uploading Image".GeneratedLog(ClassName, LogEventLevel.Information));
                }

                return uploadResult;
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Upload Image Failed: {ex}".GeneratedLog(ClassName, LogEventLevel.Error));
                throw;
            }
        }

        public async void DeletePhotoAsync(string publicId)
        {
            try
            {
                var deleteParams = new DeletionParams(publicId);

                var result = await _cloudinary.DestroyAsync(deleteParams);

                _logger.LogInformation("Remove Image Successfully".GeneratedLog(ClassName, LogEventLevel.Information));
            }
            catch (System.Exception ex)
            {
                _logger.LogWarning($"Remove Image Failed: {ex}".GeneratedLog(ClassName, LogEventLevel.Warning));
            }
        }
    }
}
