using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace KomobotV2.DataAccess
{
    public class JsonParser
    {
        private JsonConfig config;
        public JsonParser()
        {
            try
            {
                config = new JsonConfig();
                using (StreamReader r = new StreamReader("Configuration/JsonConfig.json"))
                {
                    string json = r.ReadToEnd();
                    Config = JsonConvert.DeserializeObject<JsonConfig>(json);

                }
            }
            catch(Exception e) { throw e; }
        }

        public JsonConfig Config 
            {
            //get; set;
            get { return config; }
            private set 
                {
                    if(value != null && value != config)
                    {
                        config = value;
                    }
                }
            }
    }

    
}
