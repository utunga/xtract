using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using XtractLib.CouchDB;
using XtractLib.Linq;
using XtractLib.Twitter;
using XtractLib.Words;

[TestFixture]
public class Serialize_Tests
{
   
    [Test]
    [Ignore("requires DB dependency so you have to run explictly")]
    public void TestTweetSerializeFromDB()
    {
        //setup
        IUrlExpander expander = new UrlExpander();
        Tokenizer tokenizer = new Tokenizer(expander);
        
        using (XtractDataContext db = new XtractDataContext())
        {
            db.ObjectTrackingEnabled = false;
            foreach (Tweet tw in (from tweet in db.Tweets
                                 where tweet.sample_reason.Equals(SampleReason.user_data.ToString())
                                 select tweet).Take(10))
            {
                
                Console.Out.WriteLine("JSON:" + JSON.Serialize(tw));
            }

        }
    }

    [Test]
    public void TestWordExtract()
    {
        //setup
        IUrlExpander expander = new UrlExpander();
        Tokenizer tokenizer = new Tokenizer(expander);

        List<Tweet> tweets = new List<Tweet>();
        tweets.Add( new Tweet() { 
            text =  "@steelers_munoz why can't you sleep? i'm starving, I haven't had dinner and it's 10.25 haha",
            date_scanned = DateTime.Now.ToLongTimeString(), date_tweeted = DateTime.Now.ToLongTimeString(), english_similarity = 0.5, sample_reason = SampleReason.user_data.ToString(), screen_name = "utunga", twitter_id = 9128123123});
        
        tweets.Add( new Tweet() {
            text = "RT @OMGTeenQuotez: the bad experiences i been through made me stronger..  #OMGTeenQuotez",
            date_scanned = DateTime.Now.ToLongTimeString(), date_tweeted = DateTime.Now.ToLongTimeString(), english_similarity = 0.5, sample_reason = SampleReason.user_data.ToString(), screen_name = "utunga", twitter_id = 9128123123});
        
         tweets.Add( new Tweet() {
             text = "the dog did it http://bit.ly/bkBS0o",
            date_scanned = DateTime.Now.ToLongTimeString(), date_tweeted = DateTime.Now.ToLongTimeString(), english_similarity = 0.5, sample_reason = SampleReason.user_data.ToString(), screen_name = "utunga", twitter_id = 9128123123});
        
        foreach (Tweet tw in tweets)
        {
            string screenName = tw.screen_name;
            long? twitter_id = tw.twitter_id;

            foreach (string text in tokenizer.Tokenize(tw.text))
            {
                Word word = new Word { screen_name = screenName, text = text, twitter_id = twitter_id };
                if (word.IsEntity())
                {
                    tw.AddEntity(word.text);
                    Console.Out.WriteLine("entity: " + word.text);
                }
            }
            Console.Out.WriteLine("JSON:" + JSON.Serialize(tw));
        }
    }

    [Test]
    [Ignore("requires an export to real couch dependency so you have to run explictly")]
    public void TestWordExtractPushToCouch()
    {
        //setup
        IUrlExpander expander = new UrlExpander();
        Tokenizer tokenizer = new Tokenizer(expander);
        List<Tweet> tweets = new List<Tweet>();

        PushToCouchDBReceiver receiver = new PushToCouchDBReceiver();

        tweets.Add(new Tweet()
                       {
                           text = "@steelers_munoz why can't you sleep? i'm starving, I haven't had dinner and it's 10.25 haha",
                           date_scanned = DateTime.Now.ToLongTimeString(),
                           date_tweeted = DateTime.Now.ToLongTimeString(),
                           english_similarity = 0.5,
                           sample_reason = SampleReason.user_data.ToString(),
                           screen_name = "utunga",
                           twitter_id = 9128123123
                       });

        tweets.Add(new Tweet()
                       {
                           text = "RT @OMGTeenQuotez: the bad experiences i been through made me stronger..  #OMGTeenQuotez",
                           date_scanned = DateTime.Now.ToLongTimeString(),
                           date_tweeted = DateTime.Now.ToLongTimeString(),
                           english_similarity = 0.5,
                           sample_reason = SampleReason.user_data.ToString(),
                           screen_name = "utunga",
                           twitter_id = 9128123123
                       });

        foreach (Tweet tw in tweets)
        {
            string screenName = tw.screen_name;
            long? twitter_id = tw.twitter_id;

            foreach (string text in tokenizer.Tokenize(tw.text))
            {
                Word word = new Word {screen_name = screenName, text = text, twitter_id = twitter_id};
                if (word.IsEntity())
                {
                    tw.AddEntity(word.text);
                }
            }
            receiver.Push(tw);
            Console.Out.WriteLine("JSON:" + JSON.Serialize(tw));
        }
    }
}


