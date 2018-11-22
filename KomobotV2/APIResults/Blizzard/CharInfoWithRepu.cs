using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KomobotV2.APIResults.Blizzard
{
    public class Reputation
    {
        public int id { get; set; }
        public string name { get; set; }
        public int standing { get; set; }
        public int value { get; set; }
        public int max { get; set; }
    }

    public class CharInfoWithRepu
    {
        public long lastModified { get; set; }
        public string name { get; set; }
        public string realm { get; set; }
        public string battlegroup { get; set; }
        public int @class { get; set; }
        public int race { get; set; }
        public int gender { get; set; }
        public int level { get; set; }
        public int achievementPoints { get; set; }
        public string thumbnail { get; set; }
        public string calcClass { get; set; }
        public int faction { get; set; }
        public List<Reputation> reputation { get; set; }
        public int totalHonorableKills { get; set; }
    }
}
