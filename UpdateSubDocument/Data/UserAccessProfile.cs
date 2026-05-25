using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UpdateSubDocument.Data
{
    public class UserAccessProfile
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        public string DateCreated { get; set; } = DateOnly.FromDateTime(DateTime.Now).ToString();

        public bool IsRegistered { get; set; } = false;

        public required string UserName { get; set; }

        public required string IdentityId { get; set; }

        public required bool  IsSuperAdmin { get; init; } = false;
        
        public List<PermissionsRecord> Permissions { get; set; } = [];

        /// <summary>
        /// Adds the record if it does not exist.
        /// Replaces the record if it does exist.
        /// </summary>
        /// <param name="contestAccessRecord"></param>
        public void AddPermissionsRecord(PermissionsRecord contestAccessRecord)
        {
            int index = CheckIfPermissionsRecordExists(contestAccessRecord);
            if (index == -1)
            {
                Permissions.Add(contestAccessRecord);
            }
            else
            {
                Permissions[index] = contestAccessRecord;
            }
        }

        /// <summary>
        /// Looks for matching PermissionsRecord using ContestId and OpovEventId only. 
        /// </summary>
        /// <param name="contestAccessRecord"></param>
        /// <returns> -1 if no match found, otherwise the zero-based index of matching record is returned.</returns>
        private int CheckIfPermissionsRecordExists(PermissionsRecord contestAccessRecord)
        {
            //var result = ContestsPermissions.Where(g => g.ContestId == contestAccessRecord.ContestId && g.OpovEventId == contestAccessRecord.OpovEventId).Any();
            var index = Permissions.FindIndex(g => g.ContestId == contestAccessRecord.ContestId && g.OpovEventId == contestAccessRecord.OpovEventId);
            return index; 
        }
    }

    /// <summary>
    /// Note, an OpovEvent admin should always have Contest admin privileges.
    /// Therefore, the IsContestAdmin functionality only targets non-OpovEvent admins.
    /// If the IsContestAdmin is True, then the ContestId/ContestName must also be set.
    /// </summary>

    // TODO - should I pull this class definition out into it's own file?
    public class PermissionsRecord
    {
        public string OpovEventId { get; set; }

        public string OpovEventName { get; set; }

        public string ContestId { get; set; }

        public string ContestName { get; set; }

        public bool IsContestAdmin { get; set; }

        public bool IsEventAdmin { get; set; }

        public bool IsContestant {  get; set; }

        public bool IsVoter { get; set; }
    }

}
