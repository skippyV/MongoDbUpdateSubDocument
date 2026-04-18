using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace UpdateSubDocument
{
    internal class ThirdTests
    {
        private Microsoft.Extensions.Logging.ILogger iLogger;

        public ThirdTests(ILogger<ThirdTests> iLogger)
        {
            this.iLogger = iLogger;
        }

        public void RunCode(IMongoCollection<Team> collection)
        {
            iLogger.LogInformation("In RunCode of ThirdTests");

            // https://kevsoft.net/2020/03/23/updating-arrays-in-mongodb-with-csharp.html

            // And then some array value matching
            //var filter = Builders<Member>.Filter.Eq(x => x.Id, 1)
            //    & Builders<Member>.Filter.AnyEq(x => x.Friends, 3);
            //await members.UpdateOneAsync(filter, update);

            // BELOW WORKS WITH original Friend definition of an array[int]
            // UPDATE - cannot get below code to work now. Not sure what did work before.
            // So lets just start over on updating a value in the int[]
            //var teamFilter = Builders<Team>.Filter.Eq("Id", "000000000000000000001000")
            //             & Builders<Team>.Filter.Eq("z.TeamRatings", 16);

            var filterTeam = Builders<Team>.Filter.Eq("Id", "000000000000000000001000");

            var elemMatchFilter_v1 = Builders<Team>.Filter.ElemMatch(  // elemMatchFilter_v1 does NOT work
                x => x.TeamRatings,
                Builders<int>.Filter.Eq(s => s, 16));

            var elemMatchFilter_v2 = Builders<Team>.Filter.ElemMatch(  // elemMatchFilter_v2 works
                x => x.TeamRatings,
                s => s == 16);

            var filtersAnded = Builders<Team>.Filter.And(filterTeam, elemMatchFilter_v2);

            var update = Builders<Team>.Update.Set("TeamRatings.$", 99);

            var result = collection.UpdateOne(filtersAnded, update);

            Console.WriteLine($"MatchedCount: {result.MatchedCount}");
            Console.WriteLine($"ModifiedCount: {result.ModifiedCount}");

            // Updating Team.TeamCode

            filterTeam = Builders<Team>.Filter.Eq("Id", "000000000000000000001000");
            var updateTeamCode = Builders<Team>.Update
                            .Set("TeamCode", 420);

            result = collection.UpdateOne(filterTeam, updateTeamCode); // WORKS without ArrayFilters because the target is not within an array
            Console.WriteLine($"MatchedCount: {result.MatchedCount}");
            Console.WriteLine($"ModifiedCount: {result.ModifiedCount}");

            // NOW to update a value in TeamRatings int[]

            filterTeam = Builders<Team>.Filter.Eq("Id", "000000000000000000002000");

            var arrayValueFilter = Builders<Team>.Filter.AnyEq(x => x.TeamRatings, 22);

          //  var updateRatings = Builders<Team>.Update.Set(x => x.TeamRatings.FirstMatchingElement(), 79);  // WORKS
            var updateRatings = Builders<Team>.Update.Set("TeamRatings.$", 79);  // WORKS
            result = collection.UpdateOne(filterTeam & arrayValueFilter, updateRatings); // filters are ANDED together
            Console.WriteLine($"MatchedCount: {result.MatchedCount}");
            Console.WriteLine($"ModifiedCount: {result.ModifiedCount}");

            iLogger.LogInformation("Now using ArrayFilters to target value in int[] ");

            // Now to update a value in TeamRatings int[] using an ArrayFilter           
            // These links helped me get the above ArrayFilter stuff working
            // https://www.mongodb.com/docs/manual/tutorial/use-mql-to-update-an-array/#std-label-positional-update-first-array-match
            // https://oneuptime.com/blog/post/2026-03-31-mongodb-how-to-use-identifier-with-arrayfilters-to-update-specific-a/view
            // https://thecodebuzz.com/mongodb-update-nested-array-examples/
            // https://www.mongodb.com/community/forums/t/automating-updates-to-specific-array-items-within-documents/171300/6
            // https://stackoverflow.com/questions/48876880/what-would-be-the-mongodb-c-sharp-driver-equivalent-of-the-following-query-using/48877358#48877358
            // Side Note: apparently FirstMatchingElement() is from an older version of MongoDb driver.

            // below WORKED from MONSHOSH interface
            //  db.Teams.updateOne(
            //    { _id: new ObjectId("000000000000000000002000") },
            //    { $set: { "TeamRatings.$[myIdentifier]": 555 } },
            //    {
            //       arrayFilters:
            //        [{
            //          "myIdentifier": { $eq: 79 }
            //        }]
            //    }
            // )

            var myIdentifier = "whatever";

            filterTeam = Builders<Team>.Filter.Eq("Id", "000000000000000000002000");
            
            var updateUsingArrayFilter = Builders<Team>.Update.Set($"TeamRatings.$[{myIdentifier}]", 567);

            var arrayFilter = new BsonDocumentArrayFilterDefinition<BsonDocument>(
                new BsonDocument($"{myIdentifier}", new BsonDocument("$eq", 26)));

            var arrayFilters = new List<ArrayFilterDefinition> { arrayFilter };

            var updateOptions = new UpdateOptions
            {
                ArrayFilters = arrayFilters
            };

            result = collection.UpdateOne(filterTeam, updateUsingArrayFilter, updateOptions);

            Console.WriteLine($"MatchedCount: {result.MatchedCount}");
            Console.WriteLine($"ModifiedCount: {result.ModifiedCount}");
            iLogger.LogInformation($"MatchedCount: {result.MatchedCount}");
            iLogger.LogInformation($"ModifiedCount: {result.ModifiedCount}");
        }
    }
}
