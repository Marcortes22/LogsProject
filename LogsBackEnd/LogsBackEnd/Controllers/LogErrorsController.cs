using Application.Dtos;
using Application.Interfaces;
using Domain.Collections;
using Microsoft.AspNetCore.Mvc;
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

        // GET: api/LogErrors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Log>>> GetLogs()
        {
            var logs = await _errorService.GetErrorsAsync();
            return Ok(logs);
        }


        // GET api/LogErrors/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Log>> GetLogById(string id)
        {
            var log = await _errorService.GetLogByIdAsync(id); 
            if (log == null)
            {
                return NotFound($"Log con Id = {id} no encontrado.");
            }
            return Ok(log);
        }
        // POST api/LogErrors
        [HttpPost]
        public async Task<ActionResult> CreateLog([FromBody] LogDto logDto)
        {
            await _errorService.LogErrorAsync(logDto.Message, logDto.ErrorType, logDto.Code, logDto.IsRetriable ?? false);
            return CreatedAtAction(nameof(GetLogs), new { }, "Log de error creado correctamente.");
        }

        // POST api/LogErrors/{id}/retry
        [HttpPost("{id}/retry")]
        public async Task<ActionResult> RetryLog(string id)
        {
            var success = await _errorService.RetryErrorAsync(id);
            if (success)
            {
                return Ok("Retry exitoso para el log de error.");
            }
            return BadRequest("Retry fallido o no permitido para el log especificado.");
        }

    }
}
