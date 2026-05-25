using MongoDB.Bson;
using MongoDB.Driver;
using UpdateSubDocument.Data;

namespace UpdateSubDocument
{
    internal class SecondTests
    {       
        public  void RunCode(IMongoCollection<Team> collection)
        {
            // https://stackoverflow.com/questions/31453681/mongo-update-array-element-net-driver-2-0
            // https://kevsoft.net/2020/03/23/updating-arrays-in-mongodb-with-csharp.html

            // Duplicate a Player that was already added to the collection.
            Player playerDoc = new() { PlayerName = "Gary", Id = "000000000000000000002001" };
            playerDoc.AddColor("green");
            playerDoc.AddColor("grey");

            // Use that Player for a pullUpdate - to Delete that player.
            UpdateDefinition<Team> pullUpdate = Builders<Team>.Update.Pull(teem => teem.Players, playerDoc); 
            FilterDefinition<Team> teamFilter = Builders<Team>.Filter.Eq("Id", "000000000000000000002000");

            var result = collection.UpdateOneAsync(teamFilter, pullUpdate);
            result.Wait();
            var results = result.Result;
            Console.WriteLine($"MatchedCount: {results.MatchedCount}");
            Console.WriteLine($"ModifiedCount: {results.ModifiedCount}");

            // Update Player's name
            teamFilter = Builders<Team>.Filter.Where(d => d.Id == "000000000000000000001000");

            var updatePlayerName = Builders<Team>.Update
                .Set("Players.$[p].PlayerName", "Samantha");

            var arrayFilters = new[]
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(
                        new BsonDocument("p._id", new ObjectId("000000000000000000001003") )
                    )
            };

            result = collection.UpdateOneAsync(teamFilter, updatePlayerName, new UpdateOptions { ArrayFilters = arrayFilters });
            result.Wait();
            results = result.Result;
            Console.WriteLine($"MatchedCount: {results.MatchedCount}");
            Console.WriteLine($"ModifiedCount: {results.ModifiedCount}");

            // Remove string from a Player's array of strings
            teamFilter = Builders<Team>.Filter.Where(d => d.Id == "000000000000000000002000");
            string colorToPull = "gothamGreen";

            // PullFilter vs Pull
            // The PullFilter below results with Exception:
            //    JSON reader was expecting a value but found 'gothamGreen'.
            // var updatePullNestedColorA = Builders<Team>.Update.PullFilter<Team>("Players.$[z].PlayerColors", colorToPull);

            var updatePullNestedColorB = Builders<Team>.Update.Pull("Players.$[z].PlayerColors", colorToPull);

            arrayFilters = new[]
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(
                        new BsonDocument("z._id", new ObjectId("000000000000000000002003") )
                    )
            };

            //result = collection.UpdateOneAsync(teamFilter, updatePullNestedColorA, new UpdateOptions { ArrayFilters = arrayFilters }); // See note above
            result = collection.UpdateOneAsync(teamFilter, updatePullNestedColorB, new UpdateOptions { ArrayFilters = arrayFilters });
            result.Wait();
            results = result.Result;
            Console.WriteLine($"MatchedCount: {results.MatchedCount}");
            Console.WriteLine($"ModifiedCount: {results.ModifiedCount}");
        }
    }
}
