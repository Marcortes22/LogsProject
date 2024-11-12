using Application.Dtos;
using Application.Interfaces;
using Domain.Collections;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogsBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogErrorsController : ControllerBase
    {
        private readonly IErrorService _errorService;

        public LogErrorsController(IErrorService errorService)
        {
            _errorService = errorService;
        }

        // POST api/LogErrors
        [HttpPost]
        public async Task<ActionResult> CreateLog([FromBody] LogDto logDto)
        {
            if (logDto == null) return BadRequest("Datos del log no proporcionados.");

            try
            {
                // Llama a HandleLogAsync y pasa el logDto con el objeto de compra
                var purchaseResult = await _errorService.HandleLogAsync(logDto);

                if (purchaseResult.IsSuccess)
                {
           
                    return Ok(purchaseResult);
                }

                return CreatedAtAction(nameof(GetLogById), new { id = logDto.Code }, "Log de error creado correctamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear log de error: {ex.Message}");
            }
        }

        // GET: api/LogErrors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Log>>> GetLogs()
        {
            try
            {
                var logs = await _errorService.GetErrorsAsync();
                return Ok(logs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener logs: {ex.Message}");
            }
        }

        // GET api/LogErrors/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Log>> GetLogById(string id)
        {
            try
            {
                var log = await _errorService.GetLogByIdAsync(id);
                if (log == null)
                {
                    return NotFound($"Log con Id = {id} no encontrado.");
                }
                return Ok(log);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener log por ID: {ex.Message}");
            }
        }
    }
}
