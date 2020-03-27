using System;
using System.Collections.Generic;
using System.Text;

namespace Corona.ResponseModels
{
    public class OverallCasesResponse
    {
        public int cases { get; set; }
        public int deaths { get; set; }
        public int recovered { get; set; }
        public long updated { get; set; }
    }
}
