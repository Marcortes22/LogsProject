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
    }
}
