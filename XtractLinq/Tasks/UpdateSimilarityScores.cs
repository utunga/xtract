using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using XtractLib.Linq;
using XtractLib.Trigram;
using XtractLib.Twitter;

namespace XtractLinq.Tasks
{
    public class UpdateSimilarityScores : ITask
    {
        public void Execute()
        {
            Console.Out.WriteLine("Parsing english model trigrams from source data in 'english_data' directory");
            LanguageModel english = ModelFactory.LoadModelFromFolder("english_data");
            Console.Out.WriteLine("Done parsing english model trigrams");

            int count = 0;
            using (XtractDataContext db = new XtractDataContext())
            {
                db.ObjectTrackingEnabled = false;
                var tweets = from tweet in db.Tweets
                             where tweet.sample_reason.Equals(SampleReason.user_data.ToString())
                             select tweet;
                foreach (Tweet tweet in tweets)
                {
                    LanguageModel smallModel = new LanguageModel(tweet.text);
                    double similarity = smallModel.Similarity(english);
                    Debug.Assert(similarity<=1.0d, "Similarity should never be more than 1");
                    db.SetTweetEnglishSimilarity(tweet.twitter_id, similarity);
                    if (++count%100 == 0)
                    {
                        Console.Out.WriteLine("{0}:{1}:'{2}'", tweet.twitter_id, similarity, tweet.text);
                        Console.Out.WriteLine("Updated similarity for " + count + " tweets");
                    }
                }
                Console.Out.WriteLine("Updated similarity for " + count + " tweets");
            }

        }
    }
}
