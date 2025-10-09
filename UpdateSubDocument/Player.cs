using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateSubDocument
{
    public class Player
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        public required string PlayerName { get; set; }

        public List<string> PlayerColors { get; set; }

        public void AddColor(string color)
        {
            PlayerColors.Add(color);
        }


    }
}
