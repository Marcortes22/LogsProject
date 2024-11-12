using Application.Dtos;
using Application.Hub;
using Application.Interfaces;
using Domain.Collections;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ErrorService : IErrorService
    {
        private readonly IMongoRepo _mongoRepo;
        private IHubContext<ErrorLogHub, IErrorLogHubClient> _errorHub;

        public ErrorService(IMongoRepo mongoRepo, IHubContext<ErrorLogHub, IErrorLogHubClient> errorHub)
        {
            _mongoRepo = mongoRepo;
            _errorHub = errorHub;
        }

        public async Task LogErrorAsync(string message, string errorType, string code, bool isRetriable = false)
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
            await _errorHub.Clients.All.SendErrorLogToUser(logEntry);
        }

        public async Task<bool> RetryErrorAsync(string logId)
        {
            var log = await _mongoRepo.GetLogByIdAsync(logId);
            if (log == null || !log.IsRetriable) return false;

            log.RetryCount++;

        
            bool retrySuccess = await AttemptRetryAsync(log);

            if (retrySuccess)
            {
                await _mongoRepo.DeleteLogAsync(logId); 
                return true;
            }

            await _mongoRepo.UpdateLogAsync(log); 
            return false;
        }

        public async Task<IEnumerable<Log>> GetErrorsAsync()
        {
            return await _mongoRepo.GetLogs();
        }

       
        public async Task<Log> GetLogByIdAsync(string logId)
        {
            return await _mongoRepo.GetLogByIdAsync(logId);
        }

        private Task<bool> AttemptRetryAsync(Log log)
        {
            
            return Task.FromResult(false);  
        }
    }
}
