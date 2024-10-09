using Application.Dtos;
using Domain.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IMongoRepo
    {
        Task<IEnumerable<Log>> GetLogs(LogDto logDto);
    }
}
