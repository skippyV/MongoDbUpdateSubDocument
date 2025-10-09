using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections;

// https://www.mongodb.com/docs/manual/tutorial/query-documents/

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

            var filterTeam = Builders<Team>.Filter.Eq("TeamName", "GoldDigger");

            var filterPlayer = Builders<Player>.Filter.Eq("PlayerName", "Greg");

         //   var combinedFilter = filterTeam & filterPlayer; // erroneous 

            List<string> newColors = new List<string>() { "peach", "periwinkle" };

            UpdateDefinition<Player> updateDefinition = Builders<Player>.Update.Set(doc => doc.PlayerColors, newColors); 

     //       collection.UpdateOne(combinedFilter, updateDefinition);
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

            teamDoc = new() { TeamName = "GoldDigger", TeamCode = 1148 };

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

