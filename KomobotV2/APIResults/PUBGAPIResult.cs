using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KomobotV2.APIResults
{
    public class Stats
    {
    }

    public class Attributes
    {
        public string name { get; set; }
        public string shardId { get; set; }
        public Stats stats { get; set; }
        public string createdAt { get; set; }
        public string updatedAt { get; set; }
        public string patchVersion { get; set; }
        public string titleId { get; set; }
    }

    public class Data2
    {
    }

    public class Assets
    {
        public Data2 data { get; set; }
    }

    public class Datum
    {
        public string id { get; set; }
        public string type { get; set; }
    }

    public class Matches
    {
        public List<Datum> data { get; set; }
    }

    public class Relationships
    {
        public Assets assets { get; set; }
        public Matches matches { get; set; }
    }

    public class Links
    {
        public string schema { get; set; }
        public string self { get; set; }
    }

    public class Data
    {
        public string type { get; set; }
        public string id { get; set; }
        public Attributes attributes { get; set; }
        public Relationships relationships { get; set; }
        public Links links { get; set; }
    }

    public class Links2
    {
        public string self { get; set; }
    }

    public class Meta
    {
    }

    public class PUBGPlayerResponse
    {
        public Data data { get; set; }
        public Links2 links { get; set; }
        public Meta meta { get; set; }
    }
}
