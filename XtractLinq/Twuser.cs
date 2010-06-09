using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using XtractLib.Twitter;

namespace XtractLinq
{
    public partial class Twuser : INotifyPropertyChanging, INotifyPropertyChanged
    {
        public static Twuser From(TwitterUser jsonData)
        {
            Twuser ret = new Twuser();
            ret.twitter_user_id= jsonData.id;
            ret.screen_name = jsonData.screen_name;
            ret.lang = jsonData.lang;
            ret.name = jsonData.name;
            ret.url = jsonData.url;
            ret.profile_image_url = jsonData.profile_image_url;
            ret.follower_count = jsonData.followers_count;
            return ret;
        }
    }
}
