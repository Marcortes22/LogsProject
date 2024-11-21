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
using System.Net.Http.Json;

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
    
            string logId = await LogErrorAsync(logDto.Message, logDto.ErrorType, logDto.Code, logDto.IsRetriable ?? false);
            var purchaseDto = logDto.Purchase;

         
            if (logDto.IsRetriable ?? false && purchaseDto != null)
            {
                for (int attempt = 0; attempt < 3; attempt++)
                {
                    _logger.LogInformation($"Intentando enviar compra al endpoint Retry. Intento {attempt + 1} para log ID: {logId}");

           
                    bool retrySuccess = await RetryPurchaseAsync(purchaseDto);

                    if (retrySuccess)
                    {
                        _logger.LogInformation($"Reintento exitoso para log ID: {logId}. Eliminando log y notificando al sistema externo.");

                     
                        await DeleteLogAsync(logId);
                        await NotifyExternalSystemAsync(purchaseDto);
                        return purchaseDto;
                    }

                
                    await Task.Delay(2000);
                }

                // Después de 3 intentos fallidos, marca el log como excepción
                _logger.LogWarning($"Fallaron 3 intentos de retry para Log ID: {logId}. Marcando como excepción y eliminando el objeto de compra.");
                await MarkAsExceptionAsync(logId);
            }
            else
            {
                _logger.LogWarning($"La compra para log ID: {logId} no es elegible para reintento o no se proporcionó información de compra.");
            }

            return null;
        }



        public async Task<bool> RetryPurchaseAsync(PurchaseDto purchaseDto)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var content = JsonContent.Create(purchaseDto);
                _logger.LogInformation($"Enviando JSON al endpoint Retry: {await content.ReadAsStringAsync()}");

                var response = await client.PostAsync("https://lzkf0mrp-7037.use2.devtunnels.ms/api/Compras/Retry", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Fallo al realizar el retry. Estado: {response.StatusCode}, Contenido de respuesta: {await response.Content.ReadAsStringAsync()}");
                    return false;
                }

                var responseJson = await response.Content.ReadAsStringAsync();
                var retryResult = JsonSerializer.Deserialize<bool>(responseJson);
                return retryResult;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en RetryPurchaseAsync: {ex}");
                return false;
            }
        }




        private async Task NotifyExternalSystemAsync(PurchaseDto purchaseDto)
        {
            var client = _httpClientFactory.CreateClient();
            var payload = new
            {
                Purchase = purchaseDto
            };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://lzkf0mrp-7037.use2.devtunnels.ms/api/Compras/Success", content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"Notificación enviada exitosamente.");
            }
            else
            {
                _logger.LogError($"Error al enviar la notificación. Estado: {response.StatusCode}");
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
