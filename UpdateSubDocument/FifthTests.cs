using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace UpdateSubDocument
{
    internal class FifthTests
    {
        private Microsoft.Extensions.Logging.ILogger iLogger;
        public FifthTests(ILogger<FifthTests> iLogger)
        {
            this.iLogger = iLogger;
        }
        public void RunCode(IMongoCollection<Team> collection)
        {
            iLogger.LogInformation("In RunCode of FifthTests");

            // THREE Levels - this works in MONGOSH
            //  db.Teams.updateOne(
            //    { _id: new ObjectId("000000000000000000001000") },
            //    { $set: { "Players.$[myIdentifier1].Questions.$[myIdentifier2].ImageFileIds.$[myIdentifier3]": "01234" } },
            //    {
            //      arrayFilters:
            //       [
            //         { "myIdentifier1._id": { $eq: new ObjectId("000000000000000000001001") } },
            //	       { "myIdentifier2.Number": { $eq: 2 } },
            //	       { "myIdentifier3": { $eq: "004" } },
            //	     ]
            //     }
            //  )

            string identifier1 = "firstIdentifier";
            string identifier2 = "secondIdentifier";
            string identifier3 = "thirdIdentifier";

            var filterTeam = Builders<Team>.Filter.Eq("Id", "000000000000000000001000");

            var updateStringFilter = $"Players.$[{identifier1}].Questions.$[{identifier2}].ImageFileIds.$[{identifier3}]";

            var updateUsingArrayFilter = Builders<Team>.Update.Set(updateStringFilter, "01234");

            var arrayFilter1 = new BsonDocumentArrayFilterDefinition<BsonDocument>(
                new BsonDocument($"{identifier1}._id", new BsonDocument("$eq", ObjectId.Parse("000000000000000000001001")))
                );

            var arrayFilter2 = new BsonDocumentArrayFilterDefinition<BsonDocument>(
                new BsonDocument($"{identifier2}.Number", 2));

            var arrayFilter3 = new BsonDocumentArrayFilterDefinition<BsonDocument>(
                new BsonDocument($"{identifier3}", "004")); 

            var arrayFilters = new List<ArrayFilterDefinition> { arrayFilter1, arrayFilter2, arrayFilter3 };

            var updateOptions = new UpdateOptions
            {
                ArrayFilters = arrayFilters
            };

            iLogger.LogInformation("Calling UpdateOne to update THIRD Level");
            var result = collection.UpdateOne(filterTeam, updateUsingArrayFilter, updateOptions);

            Console.WriteLine($"MatchedCount: {result.MatchedCount}");
            Console.WriteLine($"ModifiedCount: {result.ModifiedCount}");
            iLogger.LogInformation($"MatchedCount: {result.MatchedCount}");
            iLogger.LogInformation($"ModifiedCount: {result.ModifiedCount}");
        }
    }

}
