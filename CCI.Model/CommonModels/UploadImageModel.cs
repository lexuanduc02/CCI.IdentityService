using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CCI.Model.CommonModels
{
    public class UploadImageModel
    {
        [Required]
        public IFormFile File { get; set; }
        [Required]
        public string FileName { get; set; }
        [Required]
        public int With { get; set; }
        [Required]
        public int Height { get; set; }
        [Required]
        public string PublicId { get; set; }
        [Required]
        public string Folder { get; set; }
    }
}

