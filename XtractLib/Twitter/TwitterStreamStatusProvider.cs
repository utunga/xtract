using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using XtractLib.Net;

namespace XtractLib.Twitter
{
    public class TwitterStreamStatusProvider : IMessageProvider<TwitterStatus>
    {
        private ICredentials _credentials;
        public const string TWITTER_SAMPLE_URI = "http://stream.twitter.com/1/statuses/sample.json";

        public void UseCGICredentials(string username, string password)
        {
            _credentials = new NetworkCredential(username, password);
        }

        public int YieldThisMany
        {
            get;
            set;
        }
      

        public IEnumerable<TwitterStatus> GetMessages()
        {
            using (ResponseReader reader = new ResponseReader(TWITTER_SAMPLE_URI, _credentials))
            {
                string line;
                int count = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    if (count++ > YieldThisMany)
                    {
                        break;
                    }
                    yield return JSON.Deserialize<TwitterStatus>(line);
                }
            }

        }
    }
}
