using Concessionaire.WebAPI.Contexts;
using Concessionaire.WebAPI.Entities;
using Concessionaire.WebAPI.Enums;
using Concessionaire.WebAPI.Requests;
using Concessionaire.WebAPI.Services;

namespace Concessionaire.WebAPI.Repositories
{
    public class CarsRepository : ICarsRepository
    {
        private readonly ConcessionaireContext context;
        private readonly IAzureStorageService azureStorageService;
        public CarsRepository(ConcessionaireContext context, IAzureStorageService azureStorageService)
        {
            this.context = context;
            this.azureStorageService = azureStorageService;
        }

        public async Task<Car> AddAsync(CarRequest request)
        {
            var car = new Car()
            {
                Brand = request.Brand,
                Model = request.Model,
                Year = request.Year
            };

            if(request.Image != null)
            {
                 car.ImagePath = await this.azureStorageService.UploadAsync(request.Image, ContainerEnum.IMAGES); 
              }

            if (request.TechnicalDataSheet != null)
            {
                car.TechnicalDataSheetPath = await this.azureStorageService.UploadAsync(request.TechnicalDataSheet, ContainerEnum.DOCUMENTS);
            }

            this.context.Add(car);
            this.context.SaveChanges();

            return car;
        }

        public async Task<Car> AddAsyncV2(CarRequest request) 
        {
            var car = new Car()
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
                car.ImagePath = uploadResponse.BlobUrl;
                car.ImageGuid = uploadResponse.BlobGuid;
            }

            if (request.TechnicalDataSheet != null)
            {
                var uploadResponse = await this.azureStorageService.UploadAsyncV2(request.TechnicalDataSheet, ContainerEnum.DOCUMENTS);
                car.TechnicalDataSheetPath = uploadResponse.BlobUrl;
                car.TechnicalDataSheetGuid = uploadResponse.BlobGuid;
            }

            // Guardar en la base de datos
            this.context.Add(car);
            await this.context.SaveChangesAsync();

            return car;
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
            var car = this.context.Cars.Find(id);
            if(car != null)
            {
                if(!string.IsNullOrEmpty(car.ImagePath))
                {
                    await this.azureStorageService.DeleteAsync(ContainerEnum.IMAGES, car.ImagePath);
                }

                if (!string.IsNullOrEmpty(car.TechnicalDataSheetPath))
                {
                    await this.azureStorageService.DeleteAsync(ContainerEnum.DOCUMENTS, car.TechnicalDataSheetPath);
                }

                this.context.Remove(car);
                this.context.SaveChanges();
            }
        }

        public async Task<Car> UpdateAsync(int id, CarRequest request)
        {
            var car = this.context.Cars.Find(id);
            if (car != null)
            {
                car.Brand = request.Brand;
                car.Model = request.Model;
                car.Year = request.Year;

                if (request.Image != null)
                {
                    car.ImagePath = await this.azureStorageService.UploadAsync(request.Image, ContainerEnum.IMAGES, car.ImagePath);
                }

                if (request.TechnicalDataSheet != null)
                {
                    car.TechnicalDataSheetPath = await this.azureStorageService.UploadAsync(request.TechnicalDataSheet, ContainerEnum.DOCUMENTS, car.TechnicalDataSheetPath);
                }

                this.context.Update(car);
                this.context.SaveChanges();
            }

            return car;
        }
    }

    public interface ICarsRepository
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
