using BlobStorage.WebAPI.Enums;
using BlobStorage.WebAPI.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace BlobStorage.WebAPI.Requests
{
    public class CarRequest
    {
        [Required]
        public string Brand { get; set; }
        [Required]
        public string Model { get; set; }
        [Required]
        public int Year { get; set; }

        [MaxFileSize(1 * 1024 * 1024)]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" })]
        public IFormFile Image { get; set; }

        [MaxFileSize(4 * 1024 * 1024)]
        [AllowedExtensions(new string[] { ".pdf" })]
        public IFormFile TechnicalDataSheet { get; set; }
    }

    public class BlobUploadResponse
    {
        public string BlobGuid { get; set; }
        public string BlobUrl { get; set; }

        public string Image { get; set; }

        public string FileName { get; set; }
    }

    public class BlobUploadResponse64
    {
        public string BlobUrl { get; set; }
    }

    public class UploadBase64Request
    {
        public string FileBase64 { get; set; } // Archivo codificado en Base64
        public string FileName { get; set; }  // Nombre del archivo con extensión
        public ContainerEnum Container { get; set; } // Contenedor donde se almacenará
    }
}
