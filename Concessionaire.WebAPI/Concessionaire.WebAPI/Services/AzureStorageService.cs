using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BlobStorage.WebAPI.Enums;
using BlobStorage.WebAPI.Requests;
 

namespace BlobStorage.WebAPI.Services
{
    public class AzureStorageService : IAzureStorageService
    {
        private readonly string azureStorageConnectionString;
        private readonly string azureStorageContainerName;

        public AzureStorageService(IConfiguration configuration)
        {
            this.azureStorageConnectionString = configuration.GetValue<string>("AzureStorageConnectionString");
            this.azureStorageContainerName = configuration.GetValue<string>("ContainerName");
        }

        public async Task DeleteAsync(ContainerEnum container, string blobFilename)
        {
            var containerName = Enum.GetName(typeof(ContainerEnum), container).ToLower();
            var blobContainerClient = new BlobContainerClient(this.azureStorageConnectionString, containerName);
            var blobClient = blobContainerClient.GetBlobClient(blobFilename);

            try
            {
                await blobClient.DeleteAsync();
            }
            catch
            {
            }
        }

        /// <summary>
        /// This method uploads a file submitted with the request
        /// </summary>
        /// <param name="file">File for upload</param>
        /// <param name="container">Container where to submit the file</param>
        /// <param name="blobName">Blob name to update</param>
        /// <returns>File name saved in Blob contaienr</returns>
        public async Task<BlobUploadResponse> UploadAsyncV2(IFormFile file, ContainerEnum container, string blobName = null)
        {
            if (file.Length == 0) return null;

            var containerName = Enum.GetName(typeof(ContainerEnum), container).ToLower();

            var blobContainerClient = new BlobContainerClient(this.azureStorageConnectionString, azureStorageContainerName);

            // Genera un GUID para este blob
            var blobGuid = Guid.NewGuid().ToString();

            if (string.IsNullOrEmpty(blobName))
            {
                // Generar el GUID con la extensión del archivo
                blobName = Path.GetFileName(file.FileName);
                blobName = $"{containerName}/{blobName}";
            }

            var blobClient = blobContainerClient.GetBlobClient(blobName);

            var blobHttpHeader = new BlobHttpHeaders { ContentType = file.ContentType };

            // Subir el archivo
            await using (Stream stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = blobHttpHeader });
            }

            // Retornar un objeto UploadResponse con la URL y el GUID
            return new BlobUploadResponse
            {
                BlobUrl = blobClient.Uri.ToString(),  // URL completa del blob
                BlobGuid = blobGuid                   // El nombre del blob (que contiene el GUID)
            };
        }

        /// <summary>
        /// This method uploads a file submitted with the request
        /// </summary>
        /// <param name="file">File for upload</param>
        /// <param name="container">Container where to submit the file</param>
        /// <param name="blobName">Blob name to update</param>
        /// <returns>File name saved in Blob contaienr</returns>
        public async Task<string> UploadAsync(IFormFile file, ContainerEnum container, string blobName = null)
        {
            if (file.Length == 0) return null;

            var containerName = Enum.GetName(typeof(ContainerEnum), container).ToLower();

            // CYE var blobContainerClient = new BlobContainerClient(this.azureStorageConnectionString, containerName);
            var blobContainerClient = new BlobContainerClient(this.azureStorageConnectionString, azureStorageContainerName);

            // Get a reference to the blob just uploaded from the API in a container from configuration settings
            if (string.IsNullOrEmpty(blobName))
            {
                // blobName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName); // Incluye la extensión del archivo}
                //blobName = Path.GetFileName( file.FileName) ; // Incluye la extensión del archivo
                blobName = Path.GetFileName(file.FileName);
                blobName = $"{containerName}/{blobName}";

            }

            var blobClient = blobContainerClient.GetBlobClient(blobName);

            var blobHttpHeader = new BlobHttpHeaders { ContentType = file.ContentType };

            // Open a stream for the file we want to upload
            await using (Stream stream = file.OpenReadStream())
            {
                // Upload the file async
                await blobClient.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = blobHttpHeader });
            }

            //     return blobName;
            return blobClient.Uri.ToString();
        }

        /// <summary>
        /// This method uploads a file submitted with the request
        /// </summary>
        /// <param name="fileBase64">File for upload Base 64</param>
        /// <param name="container">Container where to submit the file</param>
        /// <param name="blobName">Blob name to update</param>
        /// <returns>File name saved in Blob contaienr</returns>
        public async Task<BlobUploadResponse64> UploadBase64Async(string fileBase64, string fileName, ContainerEnum container)
        {
            if (string.IsNullOrEmpty(fileBase64) || string.IsNullOrEmpty(fileName))
                throw new ArgumentException("El archivo base64 o el nombre del archivo no pueden estar vacíos.");

            // Convertir Base64 a byte array
            var fileBytes = Convert.FromBase64String(fileBase64);

            // Obtener el nombre del contenedor
            var containerName = Enum.GetName(typeof(ContainerEnum), container)?.ToLower();
            if (string.IsNullOrEmpty(containerName))
                throw new ArgumentException("El contenedor especificado no es válido.");

            var blobContainerClient = new BlobContainerClient(this.azureStorageConnectionString, azureStorageContainerName);

            // Generar el nombre del blob con la estructura contenedor/nombreArchivo
            var blobName = $"{containerName}/{fileName}";

            var blobClient = blobContainerClient.GetBlobClient(blobName);

            // Crear el encabezado HTTP con el tipo de contenido adecuado
          //  var contentType = MimeMapping.MimeUtility.GetMimeMapping(fileName); // Utiliza una biblioteca para determinar el tipo MIME
            var contentType = MimeMapping.GetMimeType(fileName);
            var blobHttpHeader = new BlobHttpHeaders { ContentType = contentType };

            // Subir el archivo al Blob Storage
            using (var stream = new MemoryStream(fileBytes))
            {
                await blobClient.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = blobHttpHeader });
            }

            // Retornar un objeto UploadResponse con la URL
            return new BlobUploadResponse64
            {
                BlobUrl = blobClient.Uri.ToString() // URL completa del blob
            };
        }

      
    }

    public interface IAzureStorageService
    {
        /// <summary>
        /// This method uploads a file submitted with the request
        /// </summary>
        /// <param name="file">File for upload</param>
        /// <param name="container">Container where to submit the file</param>
        /// <param name="blobName">Blob name to update</param>
        /// <returns>File name saved in Blob contaienr</returns>
        Task<string> UploadAsync(IFormFile file, ContainerEnum container, string blobName = null);

        /// <summary>
        /// This method deleted a file with the specified filename
        /// </summary>
        /// <param name="file">File for upload</param>
        /// <param name="container">Container where to submit the file</param>
        /// <param name="blobName">Blob name to update</param>
        /// <returns>File name saved in Blob contaienr</returns>
        Task<BlobUploadResponse> UploadAsyncV2(IFormFile file, ContainerEnum container, string blobName = null);

        /// <summary>
        /// This method deleted a file with the specified filename
        /// </summary>
        /// <param name="container">Container where to delete the file</param>
        /// <param name="blobFilename">Filename</param>
        Task DeleteAsync(ContainerEnum container, string blobFilename);
        /// <summary>
        /// This method deleted a file with the specified filename
        /// </summary>
        /// <param name="fileBase64">File for upload Base64</param>
        /// <param name="container">Container where to submit the file</param>
        /// <param name="blobName">Blob name to update</param>
        /// <returns>File name saved in Blob contaienr</returns>
        Task<BlobUploadResponse64> UploadBase64Async(string fileBase64, string fileName, ContainerEnum container);
    }

    public static class MimeMapping
    {
        private static readonly Dictionary<string, string> MimeMappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
    {
        { ".txt", "text/plain" },
        { ".pdf", "application/pdf" },
        { ".doc", "application/msword" },
        { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
        { ".xls", "application/vnd.ms-excel" },
        { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
        { ".png", "image/png" },
        { ".jpg", "image/jpeg" },
        { ".jpeg", "image/jpeg" },
        { ".gif", "image/gif" },
        { ".csv", "text/csv" },
        { ".zip", "application/zip" },
        { ".mp4", "video/mp4" },
        // Agrega más extensiones según tus necesidades
    };

        public static string GetMimeType(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            if (extension == null)
                return "application/octet-stream"; // Valor predeterminado si no hay extensión

            return MimeMappings.TryGetValue(extension, out var mimeType) ? mimeType : "application/octet-stream";
        }
    }

}