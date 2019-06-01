using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KomobotV2.APIResults.Blizzard
{
    public class Criterion
    {
        public int id { get; set; }
        public string description { get; set; }
        public int orderIndex { get; set; }
        public int max { get; set; }
    }

    public class Achievement
    {
        public int id { get; set; }
        public string title { get; set; }
        public int points { get; set; }
        public string description { get; set; }
        public List<object> rewardItems { get; set; }
        public string icon { get; set; }
        public List<Criterion> criteria { get; set; }
        public bool accountWide { get; set; }
        public int factionId { get; set; }
    }

    public class Criteria
    {
        public int id { get; set; }
        public string description { get; set; }
        public int orderIndex { get; set; }
        public int max { get; set; }
    }

    public class Feed
    {
        public string type { get; set; }
        public object timestamp { get; set; }
        public int itemId { get; set; }
        public string context { get; set; }
        public List<object> bonusLists { get; set; }
        public Achievement achievement { get; set; }
        public bool? featOfStrength { get; set; }
        public Criteria criteria { get; set; }
        public int? quantity { get; set; }
        public string name { get; set; }
    }

    public class CharInfoWithFeedResponse
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
        public List<Feed> feed { get; set; }
        public int totalHonorableKills { get; set; }
    }
}
