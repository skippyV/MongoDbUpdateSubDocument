using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using Serilog;
using Serilog.Core;

// TODO Check list
// For Top Level class Team
// 1) Change value in the class. - Done
// 2) Change value in array of ints - Done
//    2b) Change value in array of strings
// 3) Create a List<> of primitives.
// 4) Change value in List<> of primitives. 
// 5) Repeat steps 1-4 for a specific Player of a specific Team's Players list.
// 6) Add a new class and make a list of it for each Player.
// 7) Repeat steps 1-4 for a specific subdocument in a specific Player.
//
// This testing has demonstrated that lists are treated as arrays in MonogDb
// so the code covers both scenarios.

// https://stackoverflow.com/questions/79786685/mongodb-net-updating-embedded-document-in-list-with-filters-based-on-parent-and
// https://stackoverflow.com/questions/78814121/mongodb-how-to-filter-and-update-on-a-child-of-a-child/78814123#78814123
// https://stackoverflow.com/questions/56399090/push-an-item-to-a-deeply-nested-array-in-mongodb
//
// Skippy's posts:
// https://stackoverflow.com/questions/79907980/mongodb-update-array-within-an-array-of-docs-using-id
// https://stackoverflow.com/questions/79928841/mongodb-net-update-subdocument-using-id-within-arrayfilter
// https://stackoverflow.com/questions/79927435/mongodb-net-driver-failing-to-create-nested-documents-3-levels-deep
// https://stackoverflow.com/questions/79914455/mongodb-net-to-replace-string-in-array-using-elemmatch

namespace UpdateSubDocument
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Testing updating subdocument properties");

            // Setup Serilog logging
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            //MongoClient? mongoClient = new MongoClient("mongodb://127.0.0.1:27017/");
            MongoClientSettings settings = MongoClientSettings.FromConnectionString("mongodb://127.0.0.1:27017/");
            settings.LoggingSettings = new LoggingSettings(serviceProvider.GetService<ILoggerFactory>());
            ILogger<Program>? iLoggerForProgram = serviceProvider.GetService<ILogger<Program>>();
            if (iLoggerForProgram is not null)
            {
                iLoggerForProgram!.LogInformation("Log in Progam.cs");
            }

            var mongoClient = new MongoClient(settings);

            IMongoDatabase? iMongoDatabase = mongoClient.GetDatabase("UpdateSubDocumentTesting");            

            FirstTests firstTests = new();
            IMongoCollection<Team> collection = CreateTheDocs(iMongoDatabase); // Create the Teams
            //firstAttempts.RunCode(collection);

            SecondTests secondTests = new();
            collection = CreateTheDocs(iMongoDatabase); // Re-Initialize the teams
            //secondTests.RunCode(collection);

            // Inject the Seriloger into ThirdTests class
            ILogger<ThirdTests>? iLoggerForThirdTests = serviceProvider.GetService<ILogger<ThirdTests>>();
            ThirdTests thirdTests = new ThirdTests(iLoggerForThirdTests);

            collection = CreateTheDocs(iMongoDatabase); // Re-Initialize the teams
            thirdTests.RunCode(collection);

            // can I not just inject the same logger? Why am I creating the loggers using a class definition?
            // Don't think I am structuring my logging correctly.
            FourthTests fourthTests = new FourthTests(iLoggerForThirdTests);
            fourthTests.RunCode(collection);

            FifthTests fifthTests = new FifthTests(serviceProvider.GetService<ILogger<FifthTests>>()!);
            fifthTests.RunCode(collection);
        }

        public static IMongoCollection<Team> CreateTheDocs(IMongoDatabase? iMongoDatabase)
        {
            IMongoCollection<Team> TeamsCollection;

            bool collectionExists = iMongoDatabase!.ListCollectionNames().ToList().Contains("Teams");

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
                TeamRatings = new int[] { 14, 16, 19 },
                TeamOwners = new string[] { "Jerry", "Melissa", "Rihanna" },
                TeamStats = new List<int>() { 34, 35, 36 }
            };

            Player playerDoc = new() { PlayerName = "Suzie", Id = "000000000000000000001001" };
            playerDoc.AddColor("black");
            playerDoc.AddColor("blue");

            Question question = QuestionHelper.CreateQuestion(playerDoc.Questions);
            question.Text = "Howdy question 1";
            question.AddImageFileReference("001");
            question.AddImageFileReference("002");
            playerDoc.AddQuestion(question);

            question = QuestionHelper.CreateQuestion(playerDoc.Questions); 
            question.Text = "Howdy question 2";
            question.AddImageFileReference("003");
            question.AddImageFileReference("004");
            playerDoc.AddQuestion(question);

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
                TeamRatings = new int[] { 25, 22, 26 },
                TeamOwners = new string[] { "Susan", "Bob", "Juan", "Skippy" },
                TeamStats = new List<int>() { 52, 53, 54 }
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

        private static void ConfigureServices(IServiceCollection services)
        {

            // services.AddLogging(configure => configure.AddSerilog());
            // services.AddLogging(configure => configure.AddSerilog()).AddTransient<ILogger<MyClass>>();

            Logger skippy = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.File("UpdateSubDocument-LOG.txt").CreateLogger();

            //services.AddLogging(configure => configure.AddSerilog(skippy)).AddTransient<SecondTests>(); // works too
            services.AddLogging(configure => configure.AddSerilog(skippy));
            services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Debug);
        }
    }
}

