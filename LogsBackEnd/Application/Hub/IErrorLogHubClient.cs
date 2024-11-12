using Domain.Collections;

namespace Application.Hub
{
    public interface IErrorLogHubClient
    {
        Task SendErrorLogToUser(Log errorLog);
    }
}
