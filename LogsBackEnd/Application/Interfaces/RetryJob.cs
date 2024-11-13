using Application.Interfaces;
using Application.Hub;
using Domain.Collections;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

public class RetryJob : IJob
{
    private readonly IErrorService _errorService;
    private readonly ILogger<RetryJob> _logger;
    private readonly IHubContext<ErrorLogHub, IErrorLogHubClient> _errorHub;

    public RetryJob(IErrorService errorService, ILogger<RetryJob> logger, IHubContext<ErrorLogHub, IErrorLogHubClient> errorHub)
    {
        _errorService = errorService;
        _logger = logger;
        _errorHub = errorHub;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Ejecutando trabajo de actualización de errores.");

        var logs = await _errorService.GetErrorsAsync();
        var latestLogs = logs.OrderByDescending(log => log.CreatedAt).Take(10).ToList();

     
        await _errorHub.Clients.All.SendErrorLogToUser(latestLogs);

        _logger.LogInformation("Los últimos 10 errores fueron obtenidos.");
    }
}
