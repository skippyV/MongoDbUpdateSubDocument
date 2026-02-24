using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

// https://stackoverflow.com/questions/79786685/mongodb-net-updating-embedded-document-in-list-with-filters-based-on-parent-and
// https://stackoverflow.com/questions/78814121/mongodb-how-to-filter-and-update-on-a-child-of-a-child/78814123#78814123
namespace UpdateSubDocument
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Testing updating subdocument properties");

            MongoClient? mongoClient = new MongoClient("mongodb://127.0.0.1:27017/");
            IMongoDatabase? iMongoDatabase = mongoClient.GetDatabase("BasicMongoDbTesting");

            var collection = CreateTheDocs(iMongoDatabase); // Create the Teams
            FilterDefinition<Team> filterAllDocs = Builders<Team>.Filter.Empty;
            IFindFluent<Team, Team> allDocsCollection = collection.Find(filterAllDocs);
            List<Team> allDocs = allDocsCollection.ToList();

            string GregsIdAsString = string.Empty;

            foreach (Team doc in allDocs)
            {
                Console.WriteLine(doc.TeamName);
                List<Player> players = doc.Players;
                foreach (Player player in players)
                {
                    Console.WriteLine($"Player: {player.PlayerName} :: {player.Id}");
                    if(player.PlayerName.Equals("Greg"))
                    {
                        GregsIdAsString = player.Id;
                    }
                }
            }

            List<string> newColors = new List<string>() { "peach", "periwinkle" };

            // AT THIS POINT Greg HAS COLORS gold and ganja

            var filterTeam = Builders<Team>.Filter.Eq("TeamName", "GoldDiggers");
            var filterPlayer = Builders<Player>.Filter.Eq("PlayerName", "Greg");
            var filterTeamPlayers = Builders<Team>.Filter.ElemMatch(x => x.Players, filterPlayer);
            var combinedFilter = filterTeam & filterTeamPlayers;

            // NOW replace Greg's colors with peach and periwinkle
            UpdateDefinition<Team> updateDefinition = Builders<Team>.Update.Set(doc => doc.Players.AllMatchingElements("p").PlayerColors, newColors);

            UpdateResult updateResult = collection.UpdateOne(combinedFilter, updateDefinition,
                new UpdateOptions
                {
                    ArrayFilters = new ArrayFilterDefinition[]
                    {
                        new BsonDocumentArrayFilterDefinition<Player>
                        (
                          //  new BsonDocument("p.PlayerName", "Greg")  // this works
                         // new BsonDocument("p.Id", BsonValue.Create(GregsIdAsString))  // this does NOT work
                         // new BsonDocument("p.Id", GregsIdAsString) // this also does NOT work.
                          new BsonDocument("p.Id", ObjectId.Parse(GregsIdAsString))  // recommended by Nestor - but this too did not work.
                        )
                    }
                });

            Console.WriteLine("Update results of ModifiedCount: " + updateResult.ModifiedCount);
        }

        public static IMongoCollection<Team> CreateTheDocs(IMongoDatabase? iMongoDatabase)
        {
            IMongoCollection<Team> TeamsCollection;

            bool collectionExists = iMongoDatabase.ListCollectionNames().ToList().Contains("Teams");

            if (collectionExists)
            {
                TeamsCollection = iMongoDatabase!.GetCollection<Team>("Teams");
                iMongoDatabase.DropCollection("Teams");
            }

            iMongoDatabase.CreateCollection("Teams");
            TeamsCollection = iMongoDatabase!.GetCollection<Team>("Teams");

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

