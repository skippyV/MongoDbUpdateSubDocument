using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateSubDocument
{
    internal class FourthTests
    {
        private Microsoft.Extensions.Logging.ILogger iLogger;

        public FourthTests(ILogger<ThirdTests> iLogger)
        {
            this.iLogger = iLogger;
        }

        public void RunCode(IMongoCollection<Team> collection)
        {
            iLogger.LogInformation("In RunCode of FourthTests");

            // TODO Check list
            // For Top Level class Team
            // 1) Change value in the class. - Done
            // 2) Change value in array of primitives - Done
            // 3) Create a list of primitives.
            // 4) Change value in List of primitives. 
            // 5) Repeat steps 1-4 for a specific Player of a specific Team's Players list.
            // 6) Add a new class and make a list of it for each Player.
            // 7) Repeat steps 1-4 for a specific subdocument in a specific Player.
        }
    }
}
