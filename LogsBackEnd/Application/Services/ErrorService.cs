using Application.Dtos;
using Application.Interfaces;
using Domain.Collections;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using Application.Hub;

namespace Application.Services
{
    public class ErrorService : IErrorService
    {
        private readonly IMongoRepo _mongoRepo;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ErrorService> _logger;
        private readonly IHubContext<ErrorLogHub, IErrorLogHubClient> _errorHub;

        public ErrorService(IMongoRepo mongoRepo, IHttpClientFactory httpClientFactory, ILogger<ErrorService> logger, IHubContext<ErrorLogHub, IErrorLogHubClient> errorHub)
        {
            _mongoRepo = mongoRepo;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _errorHub = errorHub;
        }

  
       
        public async Task<string> LogErrorAsync(string message, string errorType, string code, bool isRetriable = false)
        {
            var logEntry = new Log
            {
                Message = message,
                CreatedAt = DateTime.UtcNow,
                ErrorType = errorType,
                Code = code,
                RetryCount = 0,
                IsRetriable = isRetriable
            };

            await _mongoRepo.InsertLogAsync(logEntry);

            return logEntry.Id; 
        }


        public async Task<PurchaseDto> HandleLogAsync(LogDto logDto)
        {
            // Guarda el log en la base de datos y obtiene el ID
            string logId = await LogErrorAsync(logDto.Message, logDto.ErrorType, logDto.Code, logDto.IsRetriable ?? false);
            var purchaseDto = logDto.Purchase;

            if (purchaseDto == null && (logDto.IsRetriable ?? false))
            {
                _logger.LogInformation($"Intentando obtener datos de compra para log ID: {logId}");
                purchaseDto = await GetPurchaseDataAsync(logId);

                if (purchaseDto == null)
                {
                    _logger.LogWarning($"No se encontró información de compra para el log ID: {logId}. Marcando como excepción.");
                    await MarkAsExceptionAsync(logId);
                    return null;
                }
            }

            // Retry de hasta 3 intentos
            if (logDto.IsRetriable ?? false && purchaseDto != null)
            {
                for (int attempt = 0; attempt < 3; attempt++)
                {
                    if (purchaseDto.IsSuccess)
                    {
                        // Si es exitoso, elimina el log y notifica al sistema externo
                        await DeleteLogAsync(logId);
                        await NotifyExternalSystemAsync(purchaseDto, logId);
                        return purchaseDto;
                    }

                    // Espera antes de intentar nuevamente
                    await Task.Delay(2000);
                    _logger.LogInformation($"Intento {attempt + 1} de retry para Log ID: {logId}");
                }

                // Después de 3 intentos fallidos, marca el log como excepción y elimina `PurchaseDto`
                _logger.LogWarning($"Fallaron 3 intentos de retry para Log ID: {logId}. Marcado como excepción y eliminado el objeto de compra.");
                await MarkAsExceptionAsync(logId);
            }

            return null;
        }


        public async Task<PurchaseDto> GetPurchaseDataAsync(string logId)
        {
            var client = _httpClientFactory.CreateClient("TunnelClient");
            var response = await client.GetAsync($"https://<tunnel-url>/api/external/purchase-data/{logId}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning($"Fallo al obtener datos de compra para log ID: {logId}. Estado: {response.StatusCode}");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PurchaseDto>(json);
        }

        private async Task NotifyExternalSystemAsync(PurchaseDto purchaseDto, string logId)
        {
            var client = _httpClientFactory.CreateClient("TunnelClient");
            var payload = new
            {
                LogId = logId,
                Purchase = purchaseDto
            };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://<tunnel-url>/api/external/receive-purchase", content);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"Notificación enviada exitosamente para log ID: {logId}");
            }
            else
            {
                _logger.LogError($"Error al enviar la notificación para log ID: {logId}. Estado: {response.StatusCode}");
            }
        }

        public async Task MarkAsExceptionAsync(string logId)
        {
            var log = await _mongoRepo.GetLogByIdAsync(logId);
            if (log != null)
            {
                log.IsRetriable = false;
                log.ErrorType = "Excepcion";
                await _mongoRepo.UpdateLogAsync(log);
            }
        }

        public async Task<IEnumerable<Log>> GetErrorsAsync()
        {
            return await _mongoRepo.GetLogs();
        }

        public async Task<Log> GetLogByIdAsync(string logId)
        {
            return await _mongoRepo.GetLogByIdAsync(logId);
        }

        public async Task DeleteLogAsync(string logId)
        {
            await _mongoRepo.DeleteLogAsync(logId);
        }
    }
}
