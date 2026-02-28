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

            string GregsIdAsString = string.Empty;    // Record to Update
            string GeorgesIdAsString = string.Empty;  // Record to Delete
            string TeamGoldDiggersIdAsString = string.Empty;

            foreach (Team doc in allDocs)
            {
                Console.WriteLine(doc.TeamName);
                if(doc.TeamName.Equals("GoldDiggers"))
                {
                    TeamGoldDiggersIdAsString = doc.Id;
                }

                List<Player> players = doc.Players;
                foreach (Player player in players)
                {
                    Console.WriteLine($"Player: {player.PlayerName} :: {player.Id}");
                    if(player.PlayerName.Equals("Greg"))
                    {
                        GregsIdAsString = player.Id;
                    }
                    if(player.PlayerName.Equals("George"))
                    {
                        GeorgesIdAsString = player.Id;
                    }
                }
            }

            List<string> newColors = new List<string>() { "peach", "periwinkle" };

            // AT THIS POINT Greg HAS COLORS gold and ganja

            var filterTeam1 = Builders<Team>.Filter.Eq("TeamName", "GoldDiggers");
            var filterPlayer1 = Builders<Player>.Filter.Eq("PlayerName", "Greg");
            var filterTeamPlayers1 = Builders<Team>.Filter.ElemMatch(x => x.Players, filterPlayer1);
            var combinedFilter1 = filterTeam1 & filterTeamPlayers1;

            // NOW replace Greg's colors with peach and periwinkle
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

            // Now to delete a SubDocument

            // https://stackoverflow.com/questions/77609329/delete-and-return-document-in-nested-array-with-mongodb-c-sharp-driver

            var filterTeam2 = Builders<Team>.Filter.Eq("TeamName", "GoldDiggers");
            var filterPlayer2 = Builders<Player>.Filter.Eq("PlayerName", "Greg");
            var filterTeamPlayers2 = Builders<Team>.Filter.ElemMatch(x => x.Players, filterPlayer2);
            var combinedFilter2 = filterTeam2 & filterTeamPlayers2;

            UpdateResult res =  collection.UpdateOne(combinedFilter2,
            Builders<Team>.Update.PullFilter(e => e.Players, filterPlayer2)
            );

            Console.WriteLine($"MatchedCount: {res.MatchedCount}, ModifiedCount: {res.ModifiedCount}");

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

