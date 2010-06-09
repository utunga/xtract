using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XtractLib.Twitter
{
    public class TwitterStatus
    {
        // Properties
        public long? id { get; set; }
        public long? in_reply_to_user_id { get; set; }
        public string text { get; set; }
        public TwitterUser user { get; set; }
        public double english_similarity { get; set; }
        public string created_at { get; set; }
    }
}
