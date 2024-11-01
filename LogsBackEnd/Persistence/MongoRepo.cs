using Application.Dtos;
using Application.Interfaces;
using Domain.Collections;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public class MongoRepo : IMongoRepo
    {
        private readonly IMongoCollection<Log> logsCollection;
        public MongoRepo(IConfiguration configuration)
        {

        logsCollection = new MongoClient(configuration.GetConnectionString("MongoDb")).GetDatabase("logs").GetCollection<Log>("logs");

        }

        public async Task<IEnumerable<Log>> GetLogs(LogDto logDto)
        {
            var filter = Builders<Log>.Filter.Empty;

         
            if (logDto != null)
            {
                if (!string.IsNullOrEmpty(logDto.ErrorType))
                    filter &= Builders<Log>.Filter.Eq(log => log.ErrorType, logDto.ErrorType);

                if (!string.IsNullOrEmpty(logDto.Code))
                    filter &= Builders<Log>.Filter.Eq(log => log.Code, logDto.Code);

                if (logDto.IsRetriable.HasValue)
                    filter &= Builders<Log>.Filter.Eq(log => log.IsRetriable, logDto.IsRetriable.Value);

                if (!string.IsNullOrEmpty(logDto.Message))
                    filter &= Builders<Log>.Filter.Eq(log => log.Message, logDto.Message);

                if (logDto.CreatedAt != default(DateTime))
                    filter &= Builders<Log>.Filter.Eq(log => log.CreatedAt, logDto.CreatedAt);
            }

            return await logsCollection.Find(filter).ToListAsync();
        }

        public async Task InsertLogAsync(Log log)
        {
            await logsCollection.InsertOneAsync(log);
        }

   
        public async Task UpdateLogAsync(Log log)
        {
            var filter = Builders<Log>.Filter.Eq("Id", log.Id);
            await logsCollection.ReplaceOneAsync(filter, log);
        }

     
        public async Task<Log> GetLogByIdAsync(string id)
        {
            var filter = Builders<Log>.Filter.Eq("Id", id);
            return await logsCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task DeleteLogAsync(string id)
        {
            var filter = Builders<Log>.Filter.Eq("Id", id);
            await logsCollection.DeleteOneAsync(filter);
        }
    }
}

