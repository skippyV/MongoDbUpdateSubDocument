using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UpdateSubDocument
{
    public class Team
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        public required string TeamName { get; set; }

        public required int TeamCode { get; set; }

        public List<Player> Players { get; set; } = [];

        public void AddPlayer(Player player)
        {
            Players.Add(player);
        }
    }
}
