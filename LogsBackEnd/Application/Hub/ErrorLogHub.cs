using Domain.Collections;
using Microsoft.AspNetCore.SignalR;

namespace Application.Hub
{
    public class ErrorLogHub : Hub<IErrorLogHubClient>
    {
        public async Task SendErrorLogToUser(Log errorLog)
        {
            await Clients.All.SendErrorLogToUser(errorLog);
        }
    }
}
