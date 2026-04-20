using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateSubDocument
{
    internal class FourthTests
    {
        private Microsoft.Extensions.Logging.ILogger iLogger;

        public FourthTests(ILogger<ThirdTests> iLogger)
        {
            this.iLogger = iLogger;
        }

        // TODO Check list
        // For Top Level class Team
        // 1) Change value in the class. - Done
        // 2) Change value in array of ints - Done
        //    2b) Change value in array of strings
        // 3) Create a List<> of primitives.
        // 4) Change value in List<> of primitives. 
        // 5) Repeat steps 1-4 for a specific Player of a specific Team's Players list.
        // 6) Add a new class and make a list of it for each Player.
        // 7) Repeat steps 1-4 for a specific subdocument in a specific Player.
        //
        // This testing has demonstrated that lists are treated as arrays in MonogDb
        // so the code covers both scenarios.

        public void RunCode(IMongoCollection<Team> collection)
        {
            iLogger.LogInformation("In RunCode of FourthTests");

            //    2b) Change value in array of strings

            var myIdentifier = "whatever";

            var filterTeam = Builders<Team>.Filter.Eq("Id", "000000000000000000002000");

            var updateUsingArrayFilter = Builders<Team>.Update.Set($"TeamOwners.$[{myIdentifier}]", "Vincent");

            var arrayFilter = new BsonDocumentArrayFilterDefinition<BsonDocument>(
                new BsonDocument($"{myIdentifier}", new BsonDocument("$eq", "Skippy")));

            var arrayFilters = new List<ArrayFilterDefinition> { arrayFilter };

            var updateOptions = new UpdateOptions
            {
                ArrayFilters = arrayFilters
            };

            var result = collection.UpdateOne(filterTeam, updateUsingArrayFilter, updateOptions);

            Console.WriteLine($"MatchedCount: {result.MatchedCount}");
            Console.WriteLine($"ModifiedCount: {result.ModifiedCount}");
            iLogger.LogInformation($"MatchedCount: {result.MatchedCount}");
            iLogger.LogInformation($"ModifiedCount: {result.ModifiedCount}");

            // 4) Change value in List<> of primitives. 

            filterTeam = Builders<Team>.Filter.Eq("Id", "000000000000000000002000");

            updateUsingArrayFilter = Builders<Team>.Update.Set($"TeamStats.$[{myIdentifier}]", 555);

            arrayFilter = new BsonDocumentArrayFilterDefinition<BsonDocument>(
                new BsonDocument($"{myIdentifier}", new BsonDocument("$eq", 53)));

            arrayFilters = new List<ArrayFilterDefinition> { arrayFilter };

            updateOptions = new UpdateOptions
            {
                ArrayFilters = arrayFilters
            };

            result = collection.UpdateOne(filterTeam, updateUsingArrayFilter, updateOptions);

            Console.WriteLine($"MatchedCount: {result.MatchedCount}");
            Console.WriteLine($"ModifiedCount: {result.ModifiedCount}");
            iLogger.LogInformation($"MatchedCount: {result.MatchedCount}");
            iLogger.LogInformation($"ModifiedCount: {result.ModifiedCount}");

            // 5) Repeat steps 1-4 for a specific Player of a specific Team's Players list.
            //      Change value in List<> of primitives.

            // LETS TRY SIMPLE FIRST
            // db.Teams.updateOne(
            //  { _id: new ObjectId("000000000000000000002000") },
            //    { $set: { "Players.$[myIdentifier1].PlayerName": "Josh" } },
            //    {
            //       arrayFilters:
            //        [        
            //           { "myIdentifier1._id": { $eq: new ObjectId("000000000000000000002001") } }
	        //        ]
            //  }
            //)

            // Below MONGOSH command does what I'm trying to do with MongoDb.NET
            // db.Teams.updateOne(
            //  { _id: new ObjectId("000000000000000000002000") },
            //  { $set:
            //      { "Players.$[myIdentifier1].PlayerColors.$[myIdentifier2]": "purple" } },
            //      {
            //       arrayFilters:
            //        [        
            //         { "myIdentifier1._id": { $eq: new ObjectId("000000000000000000002001") } },
	        //         { "myIdentifier2": { $eq: "grey" } },
	        //        ]
            //      }
            // )

            string myIdentifier1 = "firstIdentifier";
            string myIdentifier2 = "secondIdentifier";

            filterTeam = Builders<Team>.Filter.Eq("Id", "000000000000000000002000");
            updateUsingArrayFilter = Builders<Team>.Update.Set($"Players.$[{myIdentifier1}].PlayerName", "Josh"); // let's just try 1 identifier
                

            var arrayFilter1 = new BsonDocumentArrayFilterDefinition<BsonDocument>(
                // new BsonDocument($"{myIdentifier1}.Id", "000000000000000000002001")
                // new BsonDocument($"{myIdentifier1}._id", "000000000000000000002001")
                // new BsonDocument($"{myIdentifier1}.Id", new BsonDocument("$eq", "000000000000000000002001"))
                // new BsonDocument($"{myIdentifier1}._id", new BsonDocument("$eq", "000000000000000000002001"))
                new BsonDocument($"{myIdentifier1}.PlayerName", new BsonDocument("$eq", "Gary"))
                );


            //updateUsingArrayFilter = Builders<Team>.Update.Set($"Players.$[{myIdentifier1}].PlayerColors.$[{myIdentifier2}]", "violet"); // using 2 identifiers - problem

            
            //var arrayFilter2 = new BsonDocumentArrayFilterDefinition<BsonDocument>(
            //    new BsonDocument($"{myIdentifier2}",  "grey"));

            //var arrayFilter2 = new BsonDocumentArrayFilterDefinition<BsonDocument>(
            //    new BsonDocument($"{myIdentifier2}", new BsonDocument("$eq", "grey")));

            //arrayFilters = new List<ArrayFilterDefinition> { arrayFilter1, arrayFilter2 };

            arrayFilters = new List<ArrayFilterDefinition> { arrayFilter1 };

            updateOptions = new UpdateOptions
            {
                ArrayFilters = arrayFilters
            };

            iLogger.LogInformation("In RunCode of FourthTests - Attempting UPDATE");

            result = collection.UpdateOne(filterTeam, updateUsingArrayFilter, updateOptions);

            Console.WriteLine($"MatchedCount: {result.MatchedCount}");
            Console.WriteLine($"ModifiedCount: {result.ModifiedCount}");
            iLogger.LogInformation($"MatchedCount: {result.MatchedCount}");
            iLogger.LogInformation($"ModifiedCount: {result.ModifiedCount}");

        }
    }
}
