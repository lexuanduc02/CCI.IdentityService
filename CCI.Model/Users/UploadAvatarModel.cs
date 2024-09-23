using Microsoft.AspNetCore.Http;

namespace CCI.Model.Users
{
    public class UploadAvatarModel
    {
        public Guid UserId { get; set; }
        public IFormFile File { get; set; }
    }
}
