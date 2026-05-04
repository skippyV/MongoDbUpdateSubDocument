using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;


namespace UpdateSubDocument
{
    internal class SixthTests
    {
        private Microsoft.Extensions.Logging.ILogger iLogger;

        public SixthTests(ILogger<SixthTests> iLogger)
        {
            this.iLogger = iLogger;
        }

        public void RunCode(IMongoCollection<Team> collection)
        {
            // Can we use array filters to target the List/Array element of Bucket that matches on 2 conditions ANDed?
  
            // https://www.mongodb.com/docs/manual/reference/operator/update/positional-filtered/#update-all-array-elements-that-match-multiple-conditions

            iLogger.LogInformation("In RunCode of SixthTests");
            string identifier1 = "firstIdentifier";
            string identifier2 = "secondIdentifier";
            string identifier3 = "thirdIdentifier";

            var filterTeam = Builders<Team>.Filter.Eq("Id", "000000000000000000001000");

            var updateFilterString = $"Players.$[{identifier1}].Bucket.$[{identifier2}].MyValue";

            var updateUsingArrayFilter = Builders<Team>.Update.Set(updateFilterString, 379);

            var arrayFilter1 = new BsonDocumentArrayFilterDefinition<BsonDocument>(
                new BsonDocument($"{identifier1}._id", new BsonDocument("$eq", ObjectId.Parse("000000000000000000001001")))
                );

            // this BsonDocument constructor is obsolete. Gotta use a List
            //var arrayFilterCombinedConditionsANDed = new BsonDocumentArrayFilterDefinition<BsonDocument>(
            //    new BsonDocument(new BsonElement($"{identifier2}.LevelOneId", "Fruit"),
            //                     new BsonElement($"{identifier2}.LevelTwoId", "Basic"))
            //    );

            List<BsonElement> BsonElementList = new List<BsonElement>();
            BsonElementList.Add(new BsonElement($"{identifier2}.LevelOneId", "Fruit"));
            BsonElementList.Add(new BsonElement($"{identifier2}.LevelTwoId", "Basic"));

            var arrayFilterCombinedConditionsANDed = new BsonDocumentArrayFilterDefinition<BsonDocument>(
                new BsonDocument(BsonElementList)
                );

            var arrayFilters = new List<ArrayFilterDefinition> { arrayFilter1, arrayFilterCombinedConditionsANDed };

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
