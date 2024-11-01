using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Collections;

namespace Application.Interfaces
{
    public interface IMongoRepo
    {
        Task<IEnumerable<Log>> GetLogs(); // Eliminado el parámetro LogDto
    }
}
