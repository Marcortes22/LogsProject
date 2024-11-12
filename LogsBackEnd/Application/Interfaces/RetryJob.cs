using Application.Dtos;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using Polly;
using Quartz;

public class RetryJob : IJob
{
    private readonly IErrorService _errorService;
    private readonly ILogger<RetryJob> _logger;

    public RetryJob(IErrorService errorService, ILogger<RetryJob> logger)
    {
        _errorService = errorService;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Ejecutando el retry para errores controlados.");

        var logs = await _errorService.GetErrorsAsync();
        foreach (var log in logs.Where(l => l.IsRetriable))
        {
            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning($"Intento de retry {retryCount} fallido para Log ID: {log.Id}. Esperando {timeSpan} antes del próximo intento.");
                    });

            var retrySuccessful = false;

            await retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    var purchaseResult = await _errorService.HandleLogAsync(new LogDto
                    {
                        Code = log.Id,
                        Message = log.Message,
                        ErrorType = log.ErrorType,
                        IsRetriable = log.IsRetriable
                    });

                    if (purchaseResult != null && purchaseResult.IsSuccess)
                    {
                        _logger.LogInformation($"Retry exitoso y log eliminado para Log ID: {log.Id}");
                        retrySuccessful = true;
                    }
                    else
                    {
                        _logger.LogWarning($"Retry fallido para Log ID: {log.Id}, será reintentado más tarde.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error al intentar retry para Log ID: {log.Id}: {ex.Message}", ex);
                    throw;
                }
            });

            if (!retrySuccessful)
            {
                _logger.LogWarning($"Marcando Log ID: {log.Id} como excepción después de 3 intentos fallidos.");
                await _errorService.MarkAsExceptionAsync(log.Id);
            }
        }
    }
}
