using IdentityModel.Client;
using LogsBackEnd;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration; // Importar el espacio de nombres para IConfiguration
using System.Net.Http;

namespace LogController.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<LogController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration; // Agregar IConfiguration para acceder a la configuración

        public LogController(ILogger<LogController> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration; // Asignar la configuración
        }

        [HttpGet(Name = "GetWeatherForecast")]
        [Authorize(Roles = "Dashboard")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost("request-token")]
        public async Task<IActionResult> RequestToken([FromBody] TokenRequestModel model)
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            var client = new HttpClient(handler); // Usar el cliente con validación flexible

            var tokenRequest = new PasswordTokenRequest
            {
                Address = _configuration["Authentication:TokenUrl"], // Leer URL desde la configuración
                ClientId = _configuration["Authentication:ClientId"], // Leer ClientId desde la configuración
                ClientSecret = _configuration["Authentication:ClientSecret"], // Leer ClientSecret desde la configuración
                UserName = model.UserName,
                Password = model.Password,
                Scope = "logerrores_api role_access offline_access"
            };

            var tokenResponse = await client.RequestPasswordTokenAsync(tokenRequest);

            if (tokenResponse.IsError)
            {
                return BadRequest(tokenResponse.Error);
            }

            return Ok(tokenResponse.AccessToken);
        }
    }
}
