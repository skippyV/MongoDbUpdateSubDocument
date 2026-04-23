using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateSubDocument.Data
{
    public class ImageFile
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        public bool IsPreserved { get; set; }

        public bool IsPublic { get; set; } // may change to AccessScope

        // UTC DateTime as string
        public string DateTimeUploaded { get; set; } // set by service call UploadImageFile()

        public string FileOwnerId { get; set; }

        public string FileNameOriginal { get; set; }

        public string FileName { get; set; }

        public string FileExtension { get; set; }

        public string DataFileId { get; set; }

        // ValueTuples cannot use names
        // https://stackoverflow.com/questions/46601631/c-sharp-valuetuple-properties-naming

        // Item1 of tuple is OpovEvent ID, Item2 of tuple is Contest ID
        public List<(string, string)> UsedByIdTuples = new List<(string, string)>();

        //
        /// <summary>
        ///  Item1 of input tuple is OpovEvent ID, Item2 is Contest ID.
        ///  Item2 can be an empty string indicating the image is for EventOp usage.
        /// </summary>
        /// <param name="tple"></param>
        public void AddUsedByValueTuple((string, string) tuple)
        {
            UsedByIdTuples.Add(tuple);
        }
    }

    public enum AccessScopes { publicAccess = 0, eventLevel = 1, contestLevel = 2, contestantLevel = 3 } // USAGE is TBD
}
