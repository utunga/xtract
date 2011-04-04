using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XtractLib.CouchDB;
using XtractLib.Linq;
using XtractLib.Words;

namespace XtractLinq.Tasks
{
    class PushAllDataToCouch : ITask
    {
        readonly Tokenizer _tokenizer;

        public PushAllDataToCouch(Tokenizer tokenizer)
        {
            _tokenizer = tokenizer;
        }

        public void Execute()
        {
            int count =0;
            PushToCouchDBReceiver receiver = new PushToCouchDBReceiver();
            using (XtractDataContext db = new XtractDataContext())
            {
                db.ObjectTrackingEnabled = false;
                foreach (Tweet tw in from tweet in db.Tweets
                                     where tweet.sample_reason.Equals(SampleReason.user_data.ToString())
                                     select tweet)
                {
                    string screenName = tw.screen_name;
                    long? twitter_id = tw.twitter_id;

                    foreach (string text in _tokenizer.Tokenize(tw.text))
                    {
                        Word word = new Word { screen_name = screenName, text = text, twitter_id = twitter_id };
                        if (word.IsEntity())
                        {
                            tw.AddEntity(word.text);
                        }
                    }


                    receiver.Push(tw);
                    
                    if (++count % 1000 == 0)
                    {
                        Console.Out.WriteLine("tokenized " + count + " tweets");
                    }
                }

            }
        }
        
    }
}