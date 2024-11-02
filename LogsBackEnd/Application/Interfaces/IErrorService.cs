using Application.Dtos;
using Domain.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IErrorService
    {
        Task LogErrorAsync(string message, string errorType, string code, bool isRetriable = false);
        Task<bool> RetryErrorAsync(string logId);
        Task<IEnumerable<Log>> GetErrorsAsync();

        Task<Log> GetLogByIdAsync(string logId); 
    }
}
