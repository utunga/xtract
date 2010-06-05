using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using XtractLib.Net;

namespace XtractLib.Twitter
{
    public static class RateLimitCheck
    {
        public const string TWITTER_RATE_LIMIT_URI = "http://api.twitter.com/1/account/rate_limit_status.json";
        public static TwitterRateLimitStatus GetStatus(ICredentials credentials)
        {
            using (ResponseReader reader = new ResponseReader(TWITTER_RATE_LIMIT_URI, credentials))
            {
                string json = reader.ReadToEnd();
                return JSON.Deserialize<TwitterRateLimitStatus>(json);
            }
        }

        public static void SleepTillOKToProceed(ICredentials credentials)
        {
            TwitterRateLimitStatus status =null;
            while (status == null)
            {
                try
                {
                    status = GetStatus(credentials);
                }
                catch (WebException ex)
                {
                    Console.Out.WriteLine("Error getting status:" + ex.Message);
                    Thread.Sleep(2*1000); // give it 2 seconds
                }
            }
            while (status.remaining_hits <= 1)
            {
                Console.Out.WriteLine("Down to " + status.remaining_hits + " remaining hits. Will sleep for " + status.reset_time_in_seconds + " seconds (till approx: " + status.reset_time + ")");
                Thread.Sleep(status.reset_time_in_seconds*1000);
                try
                {
                    status = GetStatus(credentials);
                }
                catch (WebException ex)
                {
                     Console.Out.WriteLine("Error getting status:" + ex.Message);
                     Thread.Sleep(2 * 1000); // give it 2 seconds
                }
            }
            Console.Out.WriteLine(status.remaining_hits + " remaining twitter API hits.");
        }

    }
}
