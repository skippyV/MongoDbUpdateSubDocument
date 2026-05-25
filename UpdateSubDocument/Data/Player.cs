using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UpdateSubDocument.Data
{
    public class Player
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        public required string PlayerName { get; set; }

        public List<string> PlayerColors { get; set; } = [];

        public List<Question> Questions { get; set; } = [];

        public List<Item> Bucket { get; set; } = [];

        public void AddColor(string color)
        {
            PlayerColors.Add(color);
        }

        public void AddQuestion(Question question)
        {
            Questions.Add(question);
        }

        public void AddItem(Item item) 
        { 
            Bucket.Add(item); 
        }

        public void RemoveQuestion(int number)
        {
            // Questions.Remove(x => x.Number == number);// nope
            var item = Questions.SingleOrDefault(x => x.Number == number);

            if (item is not null)
            {
                Questions.Remove(item);
            }            
        }
    }
}
