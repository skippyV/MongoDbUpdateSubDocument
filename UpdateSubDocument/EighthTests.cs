using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Serilog;
using UpdateSubDocument.Data;

namespace UpdateSubDocument
{
    internal class EighthTests
    {
        private Microsoft.Extensions.Logging.ILogger iLogger;
        public EighthTests(ILogger<EighthTests> iLogger)
        {
            this.iLogger = iLogger;
        }
        public void RunCode(IMongoCollection<Team> teamCollection, IMongoCollection<UserAccessProfile> profileCollection)
        {
            // Ran into issue with Opov project for SetEventAdminStatusInDbForUserAccessProfile()
            // So copied it's UserAccessProfile definition to this project. Since that DB is already in place
            // I should be able to just tweak those DB records from this project.

            FilterDefinition<Team> filterTeam = Builders<Team>.Filter
                .Eq(nameof(Team.TeamName), "SandPipers");

            bool newBoolValue = true;

            string identifier1 = "recordIdentifier";

            var updateFilterString = $"Laundry.$[{identifier1}].UseSoap";

            UpdateDefinition<Team> updateUsingArrayFilter = Builders<Team>.Update.Set(updateFilterString, newBoolValue);

            var arrayFilter1 = new BsonDocumentArrayFilterDefinition<BsonDocument>(
                new BsonDocument($"{identifier1}.name", new BsonDocument("$eq", "socks"))
                );

            var arrayFilters = new List<ArrayFilterDefinition> {arrayFilter1};
            var updateOptions = new UpdateOptions{ ArrayFilters = arrayFilters};

            var updateResult = teamCollection.UpdateOne(filterTeam, updateUsingArrayFilter, updateOptions);

            if (updateResult.ModifiedCount > 0)
            {
                Console.WriteLine("UserAccessProfile was updated in Db.");
                iLogger.LogInformation("UserAccessProfile was updated in Db.");
            }
            else
            {
                Console.WriteLine("No changes made in Db.");
                iLogger.LogInformation("No changes made in Db.");
            }

            // Now to do the UserAccessProfile test                      

            string userIdentityId = "6a106416626b7a86cc103281";
            string opovEventId = "6a106451626b7a86cc103284";

            FilterDefinition<UserAccessProfile> filterProfile = Builders<UserAccessProfile>.Filter
                .Eq(nameof(UserAccessProfile.IdentityId), userIdentityId);

            string identifier2 = "recordIdentifier";

            var updateFilterString2 = $"Permissions.$[{identifier2}].IsEventAdmin";

            //bool isAdmin = true;
            bool isAdmin = false;
            UpdateDefinition<UserAccessProfile> updateUsingArrayFilter2 = Builders<UserAccessProfile>.Update.Set(updateFilterString2, isAdmin);

            var arrayFilter2 = new BsonDocumentArrayFilterDefinition<BsonDocument>(
                new BsonDocument($"{identifier1}.OpovEventId", new BsonDocument("$eq", opovEventId))
            );

            var arrayFilters2 = new List<ArrayFilterDefinition> { arrayFilter2 };
            var updateOptions2 = new UpdateOptions { ArrayFilters = arrayFilters2 };

            iLogger.LogInformation("DDBG Testing setting profile record.");
            var updateResult2 = profileCollection.UpdateOne(filterProfile, updateUsingArrayFilter2, updateOptions2);
            iLogger.LogInformation("DDBG Returned from UpdateOne ");

            string message;
            if (updateResult.ModifiedCount > 0)
            {
                message = $"UserAccessProfile was updated in Db.";                
            }
            else
            {
                message = "Warning - No Changes made to UserAccessProfile in DB";
            }
            iLogger.LogInformation(message);

        }
    }
}
