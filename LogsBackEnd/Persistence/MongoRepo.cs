using Application.Interfaces;
using Domain.Collections;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistence
{
    public class MongoRepo : IMongoRepo
    {
        private readonly IMongoCollection<Log> logsCollection;

        public MongoRepo(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDb"));
            var database = client.GetDatabase("logs");
            logsCollection = database.GetCollection<Log>("logs");
        }

        public async Task<IEnumerable<Log>> GetLogs()
        {
            var logs = await logsCollection.Find(_ => true).ToListAsync();
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

