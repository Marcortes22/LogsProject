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
            var logs = await logsCollection.Find(log => log.Message == logDto.Message).ToListAsync();
            return logs;
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

