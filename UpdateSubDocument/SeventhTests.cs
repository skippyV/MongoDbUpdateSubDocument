using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateSubDocument
{
    internal class SeventhTests
    {
        private Microsoft.Extensions.Logging.ILogger iLogger;

        public SeventhTests(ILogger<SeventhTests> iLogger)
        {
            this.iLogger = iLogger;
        }
        public void RunCode(IMongoCollection<Team> collection)
        {
            FilterDefinition<Team> teamFilter = Builders<Team>.Filter.Eq("Id", "000000000000000000002000");

            // Revisiting PULL because I've had problems getting it to work.

            // Remove string from a Teams's array of strings
            string ownerToPull = "Juan";

            // PullFilter vs Pull
            // The PullFilter below results with Exception:
            // var updatePullNestedColorA = Builders<Team>.Update.PullFilter<Team>("Players.$[z].PlayerColors", colorToPull);


            // var updatePull = Builders<Team>.Update.Pull(nameof(Team.TeamOwners), ownerToPull); // This works

            // var updatePull = Builders<Team>.Update.PullFilter<string>((e) => e.TeamOwners, ownerToPull); // this made an exception
            // One or more errors occurred. (JSON reader was expecting a value but found 'Juan'.)

            var updatePull = Builders<Team>.Update.Pull<string>((e) => e.TeamOwners, ownerToPull); // this works too

            try
            {
                var result = collection.UpdateOneAsync(teamFilter, updatePull);
                result.Wait();
                var results = result.Result;
                Console.WriteLine($"MatchedCount: {results.MatchedCount}");
                Console.WriteLine($"ModifiedCount: {results.ModifiedCount}");
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }            
        }
    }
}
