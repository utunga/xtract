using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace XtractLib.Twitter
{
    public class TwitterStatusProvider : IMessageProvider<TwitterStatus>
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
            HttpWebRequest req = WebRequest.Create(TWITTER_SAMPLE_URI) as HttpWebRequest;
            req.ServicePoint.Expect100Continue = false;
            req.UserAgent = "TwadeMe";
            req.Timeout = 10000;
            if (_credentials != null)
            {
                req.Credentials = _credentials;
            }
         
            using (WebResponse response = req.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {

                    string line;
                    int count = 0;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (count++ > YieldThisMany)
                        {
                            break;
                        }
                        yield return JsonDeserialize<TwitterStatus>(line);
                    }
                    req.Abort();
                    response.Close();
                    reader.Close();
                }
            }
        }


        private static T JsonDeserialize<T>(string toDeserialize)
        {
            T result;
            StringReader reader = new StringReader(toDeserialize);
            JsonSerializer serializer = new JsonSerializer();
            using (JsonReader reader2 = new JsonTextReader(reader))
            {
                result = (T)serializer.Deserialize(reader2, typeof(T));
                if (reader2.Read() && (reader2.TokenType != JsonToken.Comment))
                {
                    throw new JsonSerializationException("Additional text found in JSON string after finishing deserializing object.");
                }
            }
            return result;
        }
    }
}
