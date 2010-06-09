using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using XtractLib.Twitter;

namespace XtractLinq.Tasks
{
    public class SampleStream : ITask
    {

#if DEBUG
        private const int NUM_STATUSES_TO_PULL = 100;
        private const int UPDATE_EVERY = 1;
        private const double ENGLISH_THRESHOLD = 0.1d;
#else
        private const int NUM_STATUSES_TO_PULL = 1000000;
        private const int UPDATE_EVERY = 1000;
        private const double ENGLISH_THRESHOLD = 1.5d;
#endif

        public void Execute()
        {
            int count = 0;

            TwitterStreamStatusProvider provider = new TwitterStreamStatusProvider();
            string twitter_api_username = ConfigurationManager.AppSettings["twitter_user"];
            string twitter_api_password = ConfigurationManager.AppSettings["twitter_pass"];
            provider.UseCGICredentials(twitter_api_username, twitter_api_password);
            provider.YieldThisMany = NUM_STATUSES_TO_PULL;

            Console.Out.WriteLine("Parsing english model trigrams from source data in 'english_data' directory");
            EnglishStatusProvider englishProvider = new EnglishStatusProvider(provider, "english_data");
            englishProvider.Threshold = ENGLISH_THRESHOLD;
            Console.Out.WriteLine("About to start reading from twitter - up to " + NUM_STATUSES_TO_PULL + " statuses.");

            XtractDataContext db = new XtractDataContext();
            foreach (TwitterStatus status in englishProvider.GetMessages())
            {
                string screen_name = status.user.screen_name;

                var existing = db.Twusers.Where(u => u.screen_name == screen_name);
                if (existing.Count() == 0)
                {
                    Twuser user = Twuser.From(status.user);
                    user.english_similarity = status.english_similarity;
                    db.Twusers.InsertOnSubmit(user);
                }
                else
                {
                    Twuser user = existing.First();
                    if (user.english_similarity < status.english_similarity)
                    {
                        user.english_similarity = status.english_similarity;
                    }
                }

                Tweet tweet = new Tweet();
                tweet.english_similarity = status.english_similarity;
                tweet.screen_name = status.user.screen_name;
                tweet.text = status.text;
                tweet.twitter_id = status.id;
                tweet.sample_reason = SampleReason.sample_stream.ToString();
                db.Tweets.InsertOnSubmit(tweet);
                db.SubmitChanges();

                if (count++ > UPDATE_EVERY)
                {
                    Console.Out.WriteLine("Wrote " + count + " tweets.");
                }
            }
        }
    }
}