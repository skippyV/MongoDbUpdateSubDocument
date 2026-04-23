using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace UpdateSubDocument
{
    internal class FourthTests
    {
        private Microsoft.Extensions.Logging.ILogger iLogger;

        public FourthTests(ILogger<ThirdTests> iLogger)
        {
            this.iLogger = iLogger;
        }

        public void RunCode(IMongoCollection<Team> collection)
        {
            iLogger.LogInformation("In RunCode of FourthTests");

            // Change value in array of strings in Team (top level object)

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

            // Change value in List<int> in Team (top level object)

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

            // NOW all tests will use ArrayFilter

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

            // This post showed me the correct syntax:
            //   https://stackoverflow.com/questions/79928841/mongodb-net-update-subdocument-using-id-within-arrayfilter
            var arrayFilter1 = new BsonDocumentArrayFilterDefinition<BsonDocument>(
                // new BsonDocument($"{myIdentifier1}.PlayerName", new BsonDocument("$eq", "Gary"))  // Sanity check - using PlayerName instead of ID => successful update
                // new BsonDocument($"{myIdentifier1}.Id", "000000000000000000002001")
                // new BsonDocument($"{myIdentifier1}._id", "000000000000000000002001")
                // new BsonDocument($"{myIdentifier1}.Id", new BsonDocument("$eq", "000000000000000000002001"))
                //new BsonDocument($"{myIdentifier1}._id", new BsonDocument("$eq", "000000000000000000002001"))
                new BsonDocument($"{myIdentifier1}._id", new BsonDocument("$eq", ObjectId.Parse("000000000000000000002001")))
                );

            arrayFilters = new List<ArrayFilterDefinition> { arrayFilter1 };

            updateOptions = new UpdateOptions
            {
                ArrayFilters = arrayFilters
            };

            iLogger.LogInformation("Calling UpdateOne for Player.PlayerName using an ArrayFilter");

            result = collection.UpdateOne(filterTeam, updateUsingArrayFilter, updateOptions);

            Console.WriteLine($"MatchedCount: {result.MatchedCount}");
            Console.WriteLine($"ModifiedCount: {result.ModifiedCount}");
            iLogger.LogInformation($"MatchedCount: {result.MatchedCount}");
            iLogger.LogInformation($"ModifiedCount: {result.ModifiedCount}");

            // Now to do an update with TWO identifiers

            filterTeam = Builders<Team>.Filter.Eq("Id", "000000000000000000002000");

            updateUsingArrayFilter = Builders<Team>.Update.Set($"Players.$[{myIdentifier1}].PlayerColors.$[{myIdentifier2}]", "violet");

            arrayFilter1 = new BsonDocumentArrayFilterDefinition<BsonDocument>(
                new BsonDocument($"{myIdentifier1}._id", new BsonDocument("$eq", ObjectId.Parse("000000000000000000002001")))
                );

            var arrayFilter2 = new BsonDocumentArrayFilterDefinition<BsonDocument>(
                //new BsonDocument($"{myIdentifier2}", new BsonDocument("$eq", "grey")));// This also Works
                new BsonDocument($"{myIdentifier2}", "grey")); // But this version looks more succint

            arrayFilters = new List<ArrayFilterDefinition> { arrayFilter1, arrayFilter2 };

            updateOptions = new UpdateOptions
            {
                ArrayFilters = arrayFilters
            };

            iLogger.LogInformation("Calling UpdateOne targeting a string within Player.PlayerColors");

            result = collection.UpdateOne(filterTeam, updateUsingArrayFilter, updateOptions);

            Console.WriteLine($"MatchedCount: {result.MatchedCount}");
            Console.WriteLine($"ModifiedCount: {result.ModifiedCount}");
            iLogger.LogInformation($"MatchedCount: {result.MatchedCount}");
            iLogger.LogInformation($"ModifiedCount: {result.ModifiedCount}");
        }
    }
}
