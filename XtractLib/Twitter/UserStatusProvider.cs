using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using XtractLib.Net;
using XtractLib.OAuth;

namespace XtractLib.Twitter
{
    public class UserStatusProvider : IMessageProvider<TwitterStatus>
    {
        public const int MAX_PER_USER = 3200;
        public const int MAX_TRIES_FOR_SAME_URL = 4; //some users are just 'blocked' but give 401 Unauthorized errors.. so when that happens we eventually give up on them
        public const string TWITTER_USER_TIMELINE_URI = "http://api.twitter.com/1/statuses/user_timeline.json?screen_name={0}&count={1}&page={2}";
        
        private readonly string _forScreenName;
        private readonly IResponseBuilder _responseBuilder;

        public UserStatusProvider(IResponseBuilder responseBuilder, string forTwitterScreenName)
        {
            _forScreenName = forTwitterScreenName;
            _responseBuilder = responseBuilder;
        }

        public IEnumerable<TwitterStatus> GetMessages()
        {
            const int statusesPerPage = 200; // the limit according to twitter API

            int count = 0;
            int page = 0;
            bool noMoreData = false;
            while (count < MAX_PER_USER && !noMoreData)
            {
                RateLimitCheck.SleepTillOKToProceed(_responseBuilder);
                string url = string.Format(TWITTER_USER_TIMELINE_URI, _forScreenName, statusesPerPage, page);
                TwitterStatus[] results;

                int tryForSameUrl = 0;
                string json = null;
                while (json == null)
                {
                    Thread.Sleep(TwitterHappy.RecommendedWait());
                    using (IResponseReader reader = _responseBuilder.GetResponseReader(url))
                    {
                        try
                        {
                            json = reader.ReadToEnd();
                            TwitterHappy.SeemsHappy();
                        }
                        catch (WebException ex)
                        {
                            Console.Out.WriteLine("Error getting TwitterStatus for :" + url + ex.Message);
                            TwitterHappy.SeemsGrumpy();
                        }

                        //OAuthTwitterResponseBuilder oAuthTwitter = (OAuthTwitterResponseBuilder)_responseBuilder;
                        //Console.Out.WriteLine(string.Format("ConsumerKey: {0}\n ConsumerSecret: {1}\n Token: {2}\n TokenSecret: {3}\n ",
                        //    oAuthTwitter.ConsumerKey, oAuthTwitter.ConsumerSecret, oAuthTwitter.Token, oAuthTwitter.TokenSecret));

                    }

                    if (tryForSameUrl++ > MAX_TRIES_FOR_SAME_URL)
                    {
                        TwitterHappy.SeemsHappy();
                        noMoreData= true;
                        break;
                    }
                }

                if (!noMoreData)
                {
                    results = JSON.Deserialize<TwitterStatus[]>(json);
                    if (results == null || results.Length == 0)
                    {
                        noMoreData = true;
                    }
                    else
                    {

                        foreach (TwitterStatus status in results)
                        {
                            if (count++ > MAX_PER_USER)
                            {
                                break;
                            }
                            yield return status;
                        }
                    }
                }
                page++;
            }
        }

        
    }
}
