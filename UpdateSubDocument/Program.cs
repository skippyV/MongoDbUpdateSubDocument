using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

// https://stackoverflow.com/questions/79786685/mongodb-net-updating-embedded-document-in-list-with-filters-based-on-parent-and

namespace UpdateSubDocument
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Testing updating subdocument properties");

            MongoClient? mongoClient = new MongoClient("mongodb://127.0.0.1:27017/");
            IMongoDatabase? iMongoDatabase = mongoClient.GetDatabase("BasicMongoDbTesting");

            var collection = CreateTheDocs(iMongoDatabase);

            List<string> newColors = new List<string>() { "peach", "periwinkle" };

            var filterTeam = Builders<Team>.Filter.Eq("TeamName", "GoldDiggers");
            var filterPlayer = Builders<Player>.Filter.Eq("PlayerName", "Greg");
            var filterTeamPlayers = Builders<Team>.Filter.ElemMatch(x => x.Players, filterPlayer);
            var combinedFilter = filterTeam & filterTeamPlayers;

            UpdateDefinition<Team> updateDefinition = Builders<Team>.Update.Set(doc => doc.Players.AllMatchingElements("p").PlayerColors, newColors);

            UpdateResult updateResult = collection.UpdateOne(combinedFilter, updateDefinition,
                new UpdateOptions
                {
                    ArrayFilters = new ArrayFilterDefinition[] 
                    {
                        new BsonDocumentArrayFilterDefinition<Player>( new BsonDocument("p.PlayerName", "Greg") )
                    }
                });

            Console.WriteLine("Update results of ModifiedCount: " + updateResult.ModifiedCount);
        }

        public static IMongoCollection<Team> CreateTheDocs(IMongoDatabase? iMongoDatabase)
        {
            iMongoDatabase.CreateCollection("Teams");
            IMongoCollection<Team> TeamsCollection = iMongoDatabase!.GetCollection<Team>("Teams");

            Team teamDoc = new() { TeamName = "SandPipers", TeamCode = 5567 };

            Player playerDoc = new() { PlayerName = "Suzie" };
            playerDoc.AddColor("black");
            playerDoc.AddColor("blue");
            teamDoc.AddPlayer(playerDoc);

            playerDoc = new() { PlayerName = "Sandy" };
            playerDoc.AddColor("brown");
            playerDoc.AddColor("beige");
            teamDoc.AddPlayer(playerDoc);

            playerDoc = new() { PlayerName = "Sally" };
            playerDoc.AddColor("blonde");
            playerDoc.AddColor("bronze");
            teamDoc.AddPlayer(playerDoc);

            TeamsCollection.InsertOne(teamDoc);

            teamDoc = new() { TeamName = "GoldDiggers", TeamCode = 1148 };

            playerDoc = new() { PlayerName = "Gary" };
            playerDoc.AddColor("green");
            playerDoc.AddColor("grey");
            teamDoc.AddPlayer(playerDoc);

            playerDoc = new() { PlayerName = "Greg" };
            playerDoc.AddColor("gold");
            playerDoc.AddColor("ganja");
            teamDoc.AddPlayer(playerDoc);

            playerDoc = new() { PlayerName = "George" };
            playerDoc.AddColor("gothBlack");
            playerDoc.AddColor("gothamGreen");
            teamDoc.AddPlayer(playerDoc);

            TeamsCollection.InsertOne(teamDoc);

            return TeamsCollection;
        }
    }
}

