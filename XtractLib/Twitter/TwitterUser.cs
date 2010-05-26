using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XtractLib.Twitter
{
    public class TwitterUser
    {
        public int followers_count {  get;  set; }
        public long id {  get;  set; }
        public string lang {  get;  set; }
        public string name {  get;  set; }
        public string profile_image_url {  get;  set; }
        public string screen_name {  get;  set; }
        public string url {  get;  set; }
    }
}

