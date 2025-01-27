﻿using Concessionaire.WebAPI.Repositories;
using Concessionaire.WebAPI.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Concessionaire.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        private readonly ICarsRepository carsRepository;

        public CarsController(ICarsRepository carsRepository)
        {
            this.carsRepository = carsRepository;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(this.carsRepository.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var car = this.carsRepository.GetById(id);
            if(car != null)
            {
                return Ok(car);
            }

            return NotFound();
        }

        //[HttpPost]
        //public async Task<IActionResult> Create([FromForm] CarRequest request)
        //{
        //    return Ok(await this.carsRepository.AddAsyncV2(request));
        //}

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromForm] CarRequest request)
        {
            return Ok(await this.carsRepository.UpdateAsync(id, request));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            await this.carsRepository.RemoveByIdAsync(id);
            return NoContent();
        }


        [HttpPost("upload-base64")]
        public async Task<IActionResult> UploadBase64([FromBody] UploadBase64Request request)
        {
            if (request == null || string.IsNullOrEmpty(request.FileBase64) || string.IsNullOrEmpty(request.FileName))
                return BadRequest("La solicitud es inválida. Debe incluir el archivo Base64 y el nombre del archivo.");

            // Llama a la función para procesar el archivo Base64
            var result = await this.carsRepository.AddAsyncBase64(request);

            return Ok(result);
        }
         

    }
}
