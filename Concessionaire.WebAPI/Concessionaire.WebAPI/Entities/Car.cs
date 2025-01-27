namespace Concessionaire.WebAPI.Entities
{
    public partial class Car
    {
        public int Id { get; set; }
        public string Brand { get; set; } // tipo de imagen tener presente debe ser entero
        public string Model { get; set; }
        public int Year { get; set; }
        public string ImagePath { get; set; }

        public string ImageGuid { get; set; }

        public string TechnicalDataSheetPath { get; set; }

        // Propiedad para el GUID de la hoja técnica cargada (opcional si también quieres guardar el GUID)
        public string TechnicalDataSheetGuid { get; set; }
    }

    public partial class Blob
    {
        public int Id { get; set; }
      
        public string ImagePath { get; set; }

        public string ImageGuid { get; set; }
         
    }
}
