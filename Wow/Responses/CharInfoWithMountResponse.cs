using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wow.Responses
{
    public class Collected
    {
        public string name { get; set; }
        public int spellId { get; set; }
        public int creatureId { get; set; }
        public int itemId { get; set; }
        public int qualityId { get; set; }
        public string icon { get; set; }
        public bool isGround { get; set; }
        public bool isFlying { get; set; }
        public bool isAquatic { get; set; }
        public bool isJumping { get; set; }
    }

    public class Mounts
    {
        public int numCollected { get; set; }
        public int numNotCollected { get; set; }
        public List<Collected> collected { get; set; }
    }

    public class CharInfoWithMountResponse
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
        public Mounts mounts { get; set; }
        public int totalHonorableKills { get; set; }
    }
}
