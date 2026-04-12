using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using static MongoDB.Bson.Serialization.Serializers.SerializerHelper;

namespace UpdateSubDocument
{
    internal class ThirdTests
    {
        public void RunCode(IMongoCollection<Team> collection)
        {
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

            var teamFilter = Builders<Team>.Filter.Eq("Id", "000000000000000000001000");

            var elemMatchFilter_v1 = Builders<Team>.Filter.ElemMatch(  // elemMatchFilter_v1 does NOT work
                x => x.TeamRatings,
                Builders<int>.Filter.Eq(s => s, 16));

            var elemMatchFilter_v2 = Builders<Team>.Filter.ElemMatch(  // elemMatchFilter_v2 works
                x => x.TeamRatings,
                s => s == 16);

            var filtersAnded = Builders<Team>.Filter.And(teamFilter, elemMatchFilter_v2);

            var update = Builders<Team>.Update.Set("TeamRatings.$", 99);

            var result = collection.UpdateOne(filtersAnded, update);

            Console.WriteLine($"MatchedCount: {result.MatchedCount}");
            Console.WriteLine($"ModifiedCount: {result.ModifiedCount}");

            // NEXT step is to get the BELOW code working using ArrayFilters.

            // apparently FirstMatchingElement() is from an older version of MongoDb driver.

            //var arrayFilters = new List<ArrayFilterDefinition>
            //{
            //    new BsonDocumentArrayFilterDefinition<Team>(
            //        new BsonDocument
            //        {
            //            { "t.TeamRatings", new BsonDocument { { "$gte", 16} } }
            //        })
            //};      

            teamFilter = Builders<Team>.Filter.Eq("Id", "000000000000000000001000");
            var updateTeamCode = Builders<Team>.Update
                            .Set("TeamCode", 420);

            result = collection.UpdateOne(teamFilter, updateTeamCode); // WORKS without ArrayFilters because the target is not within an array
            Console.WriteLine($"MatchedCount: {result.MatchedCount}");
            Console.WriteLine($"ModifiedCount: {result.ModifiedCount}");

            // NOW to update a value in TeamRatings int[]

            teamFilter = Builders<Team>.Filter.Eq("Id", "000000000000000000002000");

            var arrayValueFilter = Builders<Team>.Filter.AnyEq(x => x.TeamRatings, 22);

          //  var updateRatings = Builders<Team>.Update.Set(x => x.TeamRatings.FirstMatchingElement(), 79);  // WORKS
            var updateRatings = Builders<Team>.Update.Set("TeamRatings.$", 79);  // WORKS
            result = collection.UpdateOne(teamFilter & arrayValueFilter, updateRatings); // filters are ANDED together
            Console.WriteLine($"MatchedCount: {result.MatchedCount}");
            Console.WriteLine($"ModifiedCount: {result.ModifiedCount}");

            // Now to try to update the value in TeamRatings int[] using an ArrayFilter
            teamFilter = Builders<Team>.Filter.Eq("Id", "000000000000000000002001");
            // https://stackoverflow.com/questions/48876880/what-would-be-the-mongodb-c-sharp-driver-equivalent-of-the-following-query-using/48877358#48877358
            var identifier = "whatever";
            var filter = Builders<Team>.Filter.Empty;
            var updateWithArrayFilter = Builders<Team>.Update.Set(a => a.TeamRatings.AllMatchingElements(identifier), 666);
            var arrayFilter = new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument($"{identifier}.{nameof(Team.TeamRatings)}", new BsonDocument("$eq", 26)));
            var arrayFilters = new List<ArrayFilterDefinition> { arrayFilter };
            var updateOptions = new UpdateOptions
            {
                ArrayFilters = arrayFilters
            };
            
            //result = collection.UpdateMany(filter, updateWithArrayFilter, updateOptions);
            //result = collection.UpdateOne(filter, updateWithArrayFilter, updateOptions); // matched one but did not modify
            result = collection.UpdateOne(teamFilter, updateWithArrayFilter, updateOptions); // no matches at all

            Console.WriteLine($"MatchedCount: {result.MatchedCount}");
            Console.WriteLine($"ModifiedCount: {result.ModifiedCount}");

            // var arrayFilters = new List<ArrayFilterDefinition>
            //  var arrayFilters = new[]
            //BsonDocumentArrayFilterDefinition<BsonDocument>[] arrayFilters = new[]
            //{
            //    new BsonDocumentArrayFilterDefinition<BsonDocument>(
            //        new BsonDocument
            //        {
            //            { "t.TeamRatings", 22 }
            //        })
            //};

            result = collection.UpdateOne(teamFilter, updateRatings, new UpdateOptions { ArrayFilters = arrayFilters });
            //result.Wait();
            //results = result.Result;
            Console.WriteLine($"MatchedCount: {result.MatchedCount}");
            Console.WriteLine($"ModifiedCount: {result.ModifiedCount}");
            // TODO: Transpose the above code by using the Teams TeamRatings int array
        }
    }
}
