using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using UpdateSubDocument.Data;

namespace UpdateSubDocument
{
    public class Team
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        public required string TeamName { get; set; }

        public required int TeamCode { get; set; }

        public int[] TeamRatings { get; set; }

        public string[] TeamOwners { get; set; }

        public List<int> TeamStats { get; set; } = [];

        public List<LaundryItem> Laundry { get; set; } = [];

        public List<Player> Players { get; set; } = [];

        public void AddPlayer(Player player)
        {
            Players.Add(player);
        }

        public void AddLaundryItem(LaundryItem laundryItem)
        {
            Laundry.Add(laundryItem);
        }
    }

    public class LaundryItem
    {
        public string name { get; set; }
        public bool UseBleach { get; set; }

        public bool UseSoap {  get; set; }
    }
}
