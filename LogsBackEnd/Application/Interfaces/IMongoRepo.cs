﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Collections;

namespace Application.Interfaces
{
    public interface IMongoRepo
    {
        Task<IEnumerable<Log>> GetLogs(); 
        Task InsertLogAsync(Log log);
        Task UpdateLogAsync(Log log);
        Task<Log> GetLogByIdAsync(string id);
        Task DeleteLogAsync(string id);
    }
}
