using BlobStorage.WebAPI.Contexts;
using BlobStorage.WebAPI.Entities;
using BlobStorage.WebAPI.Enums;
using BlobStorage.WebAPI.Requests;
using BlobStorage.WebAPI.Services;

namespace BlobStorage.WebAPI.Repositories
{
    public class CarsRepository : IBlobsRepository
    {
        private readonly BlobStorageContext context;
        private readonly IAzureStorageService azureStorageService;
        public CarsRepository(BlobStorageContext context, IAzureStorageService azureStorageService)
        {
            this.context = context;
            this.azureStorageService = azureStorageService;
        }

        public async Task<Car> AddAsync(CarRequest request)
        {
            var blob = new Car()
            {
                Brand = request.Brand,
                Model = request.Model,
                Year = request.Year
            };

            if(request.Image != null)
            {
                 blob.ImagePath = await this.azureStorageService.UploadAsync(request.Image, ContainerEnum.IMAGES); 
              }

            if (request.TechnicalDataSheet != null)
            {
                blob.TechnicalDataSheetPath = await this.azureStorageService.UploadAsync(request.TechnicalDataSheet, ContainerEnum.DOCUMENTS);
            }

            this.context.Add(blob);
            this.context.SaveChanges();

            return blob;
        }

        public async Task<Car> AddAsyncV2(CarRequest request) 
        {
            var blob = new Car()
            {
                Brand = request.Brand,
                Model = request.Model,
                Year = request.Year
            };

            if (request.Image != null)
            {
                // Subir la imagen y obtener la respuesta (URL y GUID)
                var uploadResponse = await this.azureStorageService.UploadAsyncV2(request.Image, ContainerEnum.IMAGES);

                // Guardar la URL y el GUID en la base de datos
                blob.ImagePath = uploadResponse.BlobUrl;
                blob.ImageGuid = uploadResponse.BlobGuid;
            }

            if (request.TechnicalDataSheet != null)
            {
                var uploadResponse = await this.azureStorageService.UploadAsyncV2(request.TechnicalDataSheet, ContainerEnum.DOCUMENTS);
                blob.TechnicalDataSheetPath = uploadResponse.BlobUrl;
                blob.TechnicalDataSheetGuid = uploadResponse.BlobGuid;
            }

            // Guardar en la base de datos
            this.context.Add(blob);
            await this.context.SaveChangesAsync();

            return blob;
        }

        public async Task<Blob> AddAsyncBase64(UploadBase64Request request)
        {
            var blob = new Blob();

            if (request.FileBase64 != null)
            {
                // Subir la imagen y obtener la respuesta (URL y GUID)
                var uploadResponse = await this.azureStorageService.UploadBase64Async(request.FileBase64, request.FileName, ContainerEnum.IMAGES);

                // Guardar la URL y el GUID en la base de datos
                blob.ImagePath = uploadResponse.BlobUrl;
            }

            // Guardar en la base de datos
            this.context.Add(blob);
            await this.context.SaveChangesAsync();

            return blob;
        }

         
        public List<Car> GetAll()
        {
            return this.context.Cars.ToList();
        }

        public Car GetById(int id)
        {
            return this.context.Cars.Find(id);
        }

        public async Task RemoveByIdAsync(int id)
        {
            var blob = this.context.Cars.Find(id);
            if(blob != null)
            {
                if(!string.IsNullOrEmpty(blob.ImagePath))
                {
                    await this.azureStorageService.DeleteAsync(ContainerEnum.IMAGES, blob.ImagePath);
                }

                if (!string.IsNullOrEmpty(blob.TechnicalDataSheetPath))
                {
                    await this.azureStorageService.DeleteAsync(ContainerEnum.DOCUMENTS, blob.TechnicalDataSheetPath);
                }

                this.context.Remove(blob);
                this.context.SaveChanges();
            }
        }

        public async Task<Car> UpdateAsync(int id, CarRequest request)
        {
            var blob = this.context.Cars.Find(id);
            if (blob != null)
            {
                blob.Brand = request.Brand;
                blob.Model = request.Model;
                blob.Year = request.Year;

                if (request.Image != null)
                {
                    blob.ImagePath = await this.azureStorageService.UploadAsync(request.Image, ContainerEnum.IMAGES, blob.ImagePath);
                }

                if (request.TechnicalDataSheet != null)
                {
                    blob.TechnicalDataSheetPath = await this.azureStorageService.UploadAsync(request.TechnicalDataSheet, ContainerEnum.DOCUMENTS, blob.TechnicalDataSheetPath);
                }

                this.context.Update(blob);
                this.context.SaveChanges();
            }

            return blob;
        }
    }

    public interface IBlobsRepository
    {
        List<Car> GetAll();
        Car GetById(int id);
        Task<Car> AddAsync(CarRequest request);

        Task<Car> AddAsyncV2(CarRequest request);

        Task<Car> UpdateAsync(int id, CarRequest request);
        Task RemoveByIdAsync(int id);

        Task<Blob> AddAsyncBase64(UploadBase64Request request);
    }
}
