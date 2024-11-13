using Domain.Collections;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Hub
{
    public class ErrorLogHub : Hub<IErrorLogHubClient>
    {
        public async Task SendErrorLogToUser(IEnumerable<Log> logs)
        {
            await Clients.All.SendErrorLogToUser(logs);
        }
    }
}
