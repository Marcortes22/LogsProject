using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IErrorService _errorService;

        public PurchaseService(IErrorService errorService)
        {
            _errorService = errorService;
        }

        public async Task<bool> ProcessPurchaseAsync()
        {
       
            bool hasFunds = false;  

            if (!hasFunds)
            {
                await _errorService.LogErrorAsync("Fondos insuficientes", "BusinessConstraint", "INSUFFICIENT_FUNDS", true);
                return false;
            }

        
            return true;
        }
    }
}
