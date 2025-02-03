using BlobStorage.WebAPI.Repositories;
using BlobStorage.WebAPI.Requests;
using Microsoft.AspNetCore.Mvc;

namespace BlobStorage.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlobController : ControllerBase
    {
        private readonly IBlobsRepository blobsRepository;

        public BlobController(IBlobsRepository blobsRepository)
        {
            this.blobsRepository = blobsRepository;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(this.blobsRepository.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var blob = this.blobsRepository.GetById(id);
            if(blob != null)
            {
                return Ok(blob);
            }

            return NotFound();
        }

        //[HttpPost]
        //public async Task<IActionResult> Create([FromForm] CarRequest request)
        //{
        //    return Ok(await this.blobsRepository.AddAsyncV2(request));
        //}

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromForm] CarRequest request)
        {
            return Ok(await this.blobsRepository.UpdateAsync(id, request));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            await this.blobsRepository.RemoveByIdAsync(id);
            return NoContent();
        }


        [HttpPost("upload-base64")]
        public async Task<IActionResult> UploadBase64([FromBody] UploadBase64Request request)
        {
            if (request == null || string.IsNullOrEmpty(request.FileBase64) || string.IsNullOrEmpty(request.FileName))
                return BadRequest("La solicitud es inválida. Debe incluir el archivo Base64 y el nombre del archivo.");

            // Llama a la función para procesar el archivo Base64
            var result = await this.blobsRepository.AddAsyncBase64(request);

            return Ok(result);
        }
         

    }
}
