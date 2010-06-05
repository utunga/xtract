using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XtractLib.Twitter
{
    public class TwitterRateLimitStatus
    {
        public int reset_time_in_seconds { get; set; }
        public int remaining_hits { get; set; }
        public string reset_time { get; set; }
        public int hourly_limit { get; set; }
    }
}

