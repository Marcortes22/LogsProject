using Application.Dtos;
using Domain.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IErrorService
    {
  
        Task<string> LogErrorAsync(string message, string errorType, string code, bool isRetriable = false);

  
        Task<PurchaseDto> HandleLogAsync(LogDto logDto);

   
        Task<IEnumerable<Log>> GetErrorsAsync();

    
        Task<Log> GetLogByIdAsync(string logId);

    
        Task DeleteLogAsync(string logId);

       
        Task MarkAsExceptionAsync(string logId);
    }
}
