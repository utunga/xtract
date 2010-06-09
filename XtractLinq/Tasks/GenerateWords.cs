using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XtractLib.Words;

namespace XtractLinq.Tasks
{
    class GenerateWords : ITask
    {
        readonly Tokenizer _tokenizer;

        public GenerateWords(Tokenizer tokenizer)
        {
            _tokenizer = tokenizer;
        }

        public void Execute()
        {
            int count =0;
            using (XtractDataContext db = new XtractDataContext())
            {
                db.ObjectTrackingEnabled = false;
                foreach (Tweet tw in from tweet in db.Tweets
                                     where tweet.sample_reason.Equals(SampleReason.user_data.ToString())
                                     select tweet)
                {
                    string screenName = tw.screen_name;
                    long? twitter_id = tw.twitter_id;
                    using (XtractDataContext dbWrite = new XtractDataContext())
                    {
                        foreach (string text in _tokenizer.Tokenize(tw.text))
                        {
                            Word word = new Word { screen_name = screenName, text = text, twitter_id = twitter_id };
                            dbWrite.Words.InsertOnSubmit(word);
                        }
                        dbWrite.SubmitChanges();
                        if (++count % 100 == 0)
                        {
                            Console.Out.WriteLine("tokenized " + count + " tweets");
                        }
                    }
                }
            }
        }
        
    }
}