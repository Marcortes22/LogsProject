using Application.Dtos;
using Application.Interfaces;
using Domain.Collections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Hub;
using Microsoft.AspNetCore.Authorization;

namespace LogsBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogErrorsController : ControllerBase
    {
        private readonly IErrorService _errorService;
        private readonly IHubContext<ErrorLogHub, IErrorLogHubClient> _errorHub;

        public LogErrorsController(IErrorService errorService, IHubContext<ErrorLogHub, IErrorLogHubClient> errorHub)
        {
            _errorService = errorService;
            _errorHub = errorHub;
        }

        // GET: api/LogErrors
        [HttpGet]
        [Authorize(Roles = "LogErrores")]
        public async Task<ActionResult<IEnumerable<Log>>> GetLogs()
        {
            try
            {
                var logs = await _errorService.GetErrorsAsync();

            
                await _errorHub.Clients.All.SendErrorLogToUser(logs);

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

        // POST api/LogErrors
        [HttpPost]
        public async Task<ActionResult> CreateLog([FromBody] LogDto logDto)
        {
            if (logDto == null) return BadRequest("Datos del log no proporcionados.");

            try
            {
                var purchaseResult = await _errorService.HandleLogAsync(logDto);

                if (purchaseResult?.IsSuccess == true)
                {
                    return Ok(purchaseResult);
                }

                return Created("/api/LogErrors/" + logDto.Code, "Log de error creado correctamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear log de error: {ex.Message}");
            }
        }
    }
}
