using Application.Dtos;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
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

            var logs = await _errorService.GetErrorsAsync(new LogDto { IsRetriable = true });
            foreach (var log in logs)
            {
                try
                {
                    await _errorService.RetryErrorAsync(log.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error al intentar retry para Log ID: {log.Id}", ex);
                }
            }
        }
    }
}
