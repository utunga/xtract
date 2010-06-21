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
        public enum DownloadStatus { NoDataFound, DataDownloaded }
        public void Execute()
        {
            
            int overallCount = 0;
            using (XtractDataContext dbRead = new XtractDataContext())
            {
                dbRead.ObjectTrackingEnabled = false;
                foreach (Twuser user in from twuser in dbRead.Twusers
                                        where ((twuser.id % 10 == 0) &&
                                               (twuser.last_parse_status==null))
                                        orderby twuser.english_similarity descending
                                        select twuser)
                {

                    string screen_name = user.screen_name; 
                    if (CheckForExisting(screen_name)) continue; 
                    
                    OAuthTwitterResponseBuilder oAuthTwitter = new OAuthTwitterResponseBuilder();
                    UserStatusProvider provider = new UserStatusProvider(oAuthTwitter, user.screen_name);
                    Console.Out.WriteLine("About to request data for @" + user.screen_name);
                    DateTime nowish = DateTime.UtcNow;
                    int count = 0;
                    using (XtractDataContext dbWrite = new XtractDataContext())
                    {
                        foreach (TwitterStatus status in provider.GetMessages())
                        {
                            long twitter_id = status.id.Value;
                            var existing = dbWrite.Tweets.Where(tw => tw.twitter_id == twitter_id);
                            if (existing.Count() != 0) continue;

                            Tweet tweet = new Tweet();
                            //tweet.english_similarity = status.english_similarity;
                            tweet.screen_name = status.user.screen_name;
                            tweet.text = status.text;
                            tweet.twitter_id = status.id;
                            DateTime createdAt = DateUtils.UTCDateTimeFromTwitterTimeStamp(status.created_at);
                            tweet.date_tweeted = DateUtils.ISO8601TimeStampFromUTCDateTime(createdAt);
                            tweet.date_scanned = DateUtils.ISO8601TimeStampFromUTCDateTime(nowish);
                            tweet.sample_reason = SampleReason.user_data.ToString();
                            dbWrite.Tweets.InsertOnSubmit(tweet);
                            count++;
                            overallCount++;
                            if (overallCount % 100 == 0)
                            {
                                dbWrite.SubmitChanges();
                                Console.Out.WriteLine(overallCount + " tweets saved so far");
                            }
                        }
                    }

                    Console.Out.WriteLine(count + " tweets found for " + screen_name);
                    using (XtractDataContext dbWrite = new XtractDataContext())
                    {
                        var lastUser = (from twuser in dbWrite.Twusers
                                        where twuser.screen_name.Equals(screen_name)
                                        select twuser).First();

                        DownloadStatus lastStatus = (count==0)? DownloadStatus.NoDataFound : DownloadStatus.DataDownloaded;
                        lastUser.last_parse_status = lastStatus.ToString();
                        dbWrite.SubmitChanges();
                    }
                }
            }
        }

        private bool CheckForExisting(string screen_name)
        {
            using (XtractDataContext dbWrite = new XtractDataContext())
            {
                var existingCount = (from tweet in dbWrite.Tweets
                                     where tweet.screen_name.Equals(screen_name)
                                     select tweet).Count();

                if (existingCount > 5)
                {
                    Console.Out.WriteLine("Skipping retrieve for  @" + screen_name + " as we already have " + existingCount + " rows of data");

                    var user = (from twuser in dbWrite.Twusers
                                    where twuser.screen_name.Equals(screen_name)
                                    select twuser).First();

                    DownloadStatus lastStatus = DownloadStatus.DataDownloaded;
                    user.last_parse_status = lastStatus.ToString();
                    dbWrite.SubmitChanges();
                    return true;
                }
            }
            return false;
        }
    }
}
