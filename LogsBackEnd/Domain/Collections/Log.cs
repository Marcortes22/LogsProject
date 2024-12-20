﻿using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Collections
{
    public class Log
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }

        public string Message { get; set; }

        public DateTime CreatedAt { get; set; }

        public string ErrorType { get; set; }


        public string Code { get; set; }


        public int RetryCount { get; set; }

        public bool IsRetriable { get; set; } = false;
    }
}
