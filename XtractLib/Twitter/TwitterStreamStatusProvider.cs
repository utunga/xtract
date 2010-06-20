using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using XtractLib.Net;

namespace XtractLib.Twitter
{
    public class TwitterStreamStatusProvider : IMessageProvider<TwitterStatus>
    {
        public const string TWITTER_SAMPLE_URI = "http://stream.twitter.com/1/statuses/sample.json";

        private readonly IResponseBuilder _responseBuilder;
      
        public int YieldThisMany
        {
            get;
            set;
        }

        public TwitterStreamStatusProvider(IResponseBuilder responseBuilder)
        {
            _responseBuilder = responseBuilder;
        }
        public IEnumerable<TwitterStatus> GetMessages()
        {
            using (IResponseReader reader = _responseBuilder.GetResponseReader(TWITTER_SAMPLE_URI))
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
