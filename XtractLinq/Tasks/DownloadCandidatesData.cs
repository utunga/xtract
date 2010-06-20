using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Offr.Text;
using XtractLib.OAuth;
using XtractLib.Twitter;

namespace XtractLinq.Tasks
{
    public class DownloadCandidatesData : ITask
    {
        public void Execute()
        {
            
            int count = 0;
            XtractDataContext db = new XtractDataContext();
            foreach (Twuser user in from twuser in db.Twusers
                                    where (twuser.id % 10 == 0) 
                                    orderby twuser.english_similarity descending
                                    select twuser)
            {
                string screen_name = user.screen_name;
                var existingCount = (from tweet in db.Tweets
                                     where tweet.screen_name.Equals(screen_name)
                                     select tweet).Count();
                if (existingCount > 5)
                {
                    Console.Out.WriteLine("Skipping retrieve for  @" + user.screen_name + " as we already have " + existingCount + " rows of data");
                    continue;
                }

                OAuthTwitterResponseBuilder oAuthTwitter = new OAuthTwitterResponseBuilder();
                UserStatusProvider provider = new UserStatusProvider(oAuthTwitter, user.screen_name);
                Console.Out.WriteLine("About to request data for @" + user.screen_name);
                DateTime nowish = DateTime.UtcNow;
                foreach (TwitterStatus status in provider.GetMessages())
                {
                    long twitter_id = status.id.Value;
                    var existing = db.Tweets.Where(tw => tw.twitter_id == twitter_id);
                    if (existing.Count() == 0)
                    {
                        Tweet tweet = new Tweet();
                        //tweet.english_similarity = status.english_similarity;
                        tweet.screen_name = status.user.screen_name;
                        tweet.text = status.text;
                        tweet.twitter_id = status.id;
                        DateTime createdAt = DateUtils.UTCDateTimeFromTwitterTimeStamp(status.created_at);
                        tweet.date_tweeted = DateUtils.ISO8601TimeStampFromUTCDateTime(createdAt);
                        tweet.date_scanned = DateUtils.ISO8601TimeStampFromUTCDateTime(nowish);
                        tweet.sample_reason = SampleReason.user_data.ToString();
                        db.Tweets.InsertOnSubmit(tweet);
                        if (++count % 100 == 0)
                        {
                            db.SubmitChanges();
                            Console.Out.WriteLine("Saved " + count + " tweets");
                        }
                    }
                }
            }
        }
    }
}
