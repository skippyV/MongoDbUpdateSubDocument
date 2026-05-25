using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using UpdateSubDocument.Data;

namespace UpdateSubDocument
{
    internal class FirstTests
    {

        public void RunCode(IMongoCollection<Team> collection)
        {
            FilterDefinition<Team> filterAllDocs = Builders<Team>.Filter.Empty;
            IFindFluent<Team, Team> allDocsCollection = collection.Find(filterAllDocs);
            List<Team> allDocs = allDocsCollection.ToList();

            string GregsIdAsString = string.Empty;    // Record to Update
            string GeorgesIdAsString = string.Empty;  // Record to Delete
            string TeamGoldDiggersIdAsString = string.Empty;

            foreach (Team doc in allDocs)
            {
                Console.WriteLine(doc.TeamName);
                if (doc.TeamName.Equals("GoldDiggers"))
                {
                    TeamGoldDiggersIdAsString = doc.Id;
                }

                List<Player> players = doc.Players;
                foreach (Player player in players)
                {
                    Console.WriteLine($"Player: {player.PlayerName} :: {player.Id}");
                    if (player.PlayerName.Equals("Greg"))
                    {
                        GregsIdAsString = player.Id;
                    }
                    if (player.PlayerName.Equals("George"))
                    {
                        GeorgesIdAsString = player.Id;
                    }
                }
            }

            List<string> newColors = new List<string>() { "peach", "periwinkle" };

            // AT THIS POINT Greg's COLORS are initialized to gold and ganja

            var filter1Team = Builders<Team>.Filter.Eq("Id", TeamGoldDiggersIdAsString);
            var filter1Player = Builders<Player>.Filter.Eq("Id", GregsIdAsString);
            var filter1TeamPlayers1 = Builders<Team>.Filter.ElemMatch(x => x.Players, filter1Player);
            var combinedFilter1 = filter1Team & filter1TeamPlayers1;

            // NOW replace Greg's colors with a new List of colors: peach and periwinkle
            UpdateDefinition<Team> updateDefinition1 = Builders<Team>.Update.Set(doc => doc.Players.AllMatchingElements("p").PlayerColors, newColors);

            UpdateResult updateResult = collection.UpdateOne(combinedFilter1, updateDefinition1,
                new UpdateOptions
                {
                    ArrayFilters = new ArrayFilterDefinition[]
                    {
                        new BsonDocumentArrayFilterDefinition<Player>
                        (
                         new BsonDocument("p._id", ObjectId.Parse(GregsIdAsString)) // THE MAGIC SYNTAX
                        )
                    }
                });

            Console.WriteLine("Update results of ModifiedCount: " + updateResult.ModifiedCount);

            // At this point Greg's List of colors should have been changed.


            // Now to delete a SubDocument

            // https://stackoverflow.com/questions/77609329/delete-and-return-document-in-nested-array-with-mongodb-c-sharp-driver

            var filter2Team = Builders<Team>.Filter.Eq("Id", TeamGoldDiggersIdAsString);
            var filter2Player = Builders<Player>.Filter.Eq("Id", GeorgesIdAsString);
            var filter2TeamPlayers = Builders<Team>.Filter.ElemMatch(x => x.Players, filter2Player);
            var combinedFilter2 = filter2Team & filter2TeamPlayers;

            UpdateResult res = collection.UpdateOne(combinedFilter2,
                Builders<Team>.Update.PullFilter(e => e.Players, filter2Player)
            );

            Console.WriteLine($"MatchedCount: {res.MatchedCount}, ModifiedCount: {res.ModifiedCount}");
            Console.WriteLine("George Player record should now be gone");
        }
    }
}
