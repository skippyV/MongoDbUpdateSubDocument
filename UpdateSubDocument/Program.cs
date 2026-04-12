using MongoDB.Driver;

// https://stackoverflow.com/questions/79786685/mongodb-net-updating-embedded-document-in-list-with-filters-based-on-parent-and
// https://stackoverflow.com/questions/78814121/mongodb-how-to-filter-and-update-on-a-child-of-a-child/78814123#78814123
// https://stackoverflow.com/questions/56399090/push-an-item-to-a-deeply-nested-array-in-mongodb
// https://stackoverflow.com/questions/79907980/mongodb-update-array-within-an-array-of-docs-using-id
namespace UpdateSubDocument
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Testing updating subdocument properties");

            MongoClient? mongoClient = new MongoClient("mongodb://127.0.0.1:27017/");
            IMongoDatabase? iMongoDatabase = mongoClient.GetDatabase("UpdateSubDocumentTesting");            

            FirstTests firstTests = new();
            IMongoCollection<Team> collection = CreateTheDocs(iMongoDatabase); // Create the Teams
            //firstAttempts.RunCode(collection);

            SecondTests secondTests = new();
            collection = CreateTheDocs(iMongoDatabase); // Re-Initialize the teams
            //secondTests.RunCode(collection);

            ThirdTests thirdTests = new();
            collection = CreateTheDocs(iMongoDatabase); // Re-Initialize the teams
            thirdTests.RunCode(collection);

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

            Team teamDoc = new()
            {
                TeamName = "SandPipers",
                TeamCode = 5567,
                Id = "000000000000000000001000",
                TeamRatings = new int[] { 14, 16, 19 }
            };

            Player playerDoc = new() { PlayerName = "Suzie", Id = "000000000000000000001001" };
            playerDoc.AddColor("black");
            playerDoc.AddColor("blue");
            teamDoc.AddPlayer(playerDoc);

            playerDoc = new() { PlayerName = "Sandy", Id = "000000000000000000001002" };
            playerDoc.AddColor("brown");
            playerDoc.AddColor("beige");
            teamDoc.AddPlayer(playerDoc);

            playerDoc = new() { PlayerName = "Sally", Id = "000000000000000000001003" };
            playerDoc.AddColor("blonde");
            playerDoc.AddColor("bronze");
            teamDoc.AddPlayer(playerDoc);

            TeamsCollection.InsertOne(teamDoc);

            teamDoc = new() { 
                TeamName = "GoldDiggers", 
                TeamCode = 1148, 
                Id = "000000000000000000002000",
                TeamRatings = new int[] { 25, 22, 26 }
            };

            playerDoc = new() { PlayerName = "Gary", Id = "000000000000000000002001" };
            playerDoc.AddColor("green");
            playerDoc.AddColor("grey");
            teamDoc.AddPlayer(playerDoc);

            playerDoc = new() { PlayerName = "Greg", Id = "000000000000000000002002" };
            playerDoc.AddColor("gold");
            playerDoc.AddColor("ganja");
            teamDoc.AddPlayer(playerDoc);

            playerDoc = new() { PlayerName = "George", Id = "000000000000000000002003" };
            playerDoc.AddColor("gothBlack");
            playerDoc.AddColor("gothamGreen");
            teamDoc.AddPlayer(playerDoc);

            TeamsCollection.InsertOne(teamDoc);

            return TeamsCollection;
        }
    }
}

