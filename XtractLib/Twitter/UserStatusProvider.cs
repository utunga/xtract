using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using XtractLib.Net;

namespace XtractLib.Twitter
{
    public class UserStatusProvider : IMessageProvider<TwitterStatus>
    {
        private ICredentials _credentials;
        private string _forScreenName;
        public const int MAX_PER_USER = 3200;
        public const string TWITTER_USER_TIMELINE_URI = "http://api.twitter.com/1/statuses/user_timeline.json?screen_name={0}&count={1}&page={2}";

        public void UseCGICredentials(string username, string password)
        {
            _credentials = new NetworkCredential(username, password);
        }

        public UserStatusProvider(string forTwitterScreenName)
        {
            _forScreenName = forTwitterScreenName;
        }

        public IEnumerable<TwitterStatus> GetMessages()
        {
          
            string screen_name = _forScreenName;
            int page_count = 200;
            int count = 0;
            int page = 0;
            bool noMoreData = false;
            while (count < MAX_PER_USER && !noMoreData)
            {
                RateLimitCheck.SleepTillOKToProceed(_credentials);
                string url = string.Format(TWITTER_USER_TIMELINE_URI, screen_name, page_count, page);
                TwitterStatus[] results;
                using (ResponseReader reader = new ResponseReader(url, _credentials))
                {
                    string json= null;
                    while (json == null)
                    {
                        try
                        {
                            json = reader.ReadToEnd();
                        }
                        catch (WebException ex)
                        {
                            Console.Out.WriteLine("Error getting TwitterStatus for :" + url  + ex.Message);
                            Thread.Sleep(500); // give it 1/2 a second
                        }
                    } 
                    
                    results = JSON.Deserialize<TwitterStatus[]>(json);
                    if (results.Length == 0)
                    {
                        noMoreData = true;
                    }
                }
                foreach (TwitterStatus status in results)
                {
                    if (count++ > MAX_PER_USER)
                    {
                        break;
                    }
                    yield return status;
                }
                page++;
            }
        }
    }
}
