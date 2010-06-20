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

        public static TwitterRateLimitStatus GetStatus(IResponseBuilder responseBuilder)
        {
            using (IResponseReader reader = responseBuilder.GetResponseReader(TWITTER_RATE_LIMIT_URI))
            {
                string json = reader.ReadToEnd();
                return JSON.Deserialize<TwitterRateLimitStatus>(json);
            }
        }

        public static void SleepTillOKToProceed(IResponseBuilder responseBuilder)
        {
            int sleepSeconds = 60;
            TwitterRateLimitStatus status =null;
            while (status == null)
            {
                try
                {
                    status = GetStatus(responseBuilder);
                }
                catch (WebException ex)
                {
                    Console.Out.WriteLine("Error getting status:" + ex.Message);
                    Thread.Sleep(2*1000); // give it 2 seconds
                }
            }
           
            while (status.remaining_hits <= 1)
            {
               
                Console.Out.WriteLine("Down to " + status.remaining_hits + " remaining hits. Will sleep for " + sleepSeconds + " seconds (till approx: " + DateTime.Now.AddSeconds(sleepSeconds).ToString("o") + ")");
                Thread.Sleep(sleepSeconds * 1000);
                try
                {
                    status = GetStatus(responseBuilder);
                }
                catch (WebException ex)
                {
                     Console.Out.WriteLine("Error getting status:" + ex.Message);
                     Thread.Sleep(2 * 1000); // give it 2 seconds
                }

                sleepSeconds = (int)Math.Floor(sleepSeconds * 1.2);
            }
            Console.Out.WriteLine(status.remaining_hits + " remaining twitter API hits.");
        }

    }
}
