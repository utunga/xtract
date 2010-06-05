using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace XtractLib.Twitter
{
    public class UserStatusProvider : IMessageProvider<TwitterStatus>
    {
        private ICredentials _credentials;
        private long _twitterUserID;
        public const string TWITTER_USER_URI = "http://stream.twitter.com/1/statuses/sample.json";

        public void UseCGICredentials(string username, string password)
        {
            _credentials = new NetworkCredential(username, password);
        }

        public UserStatusProvider(TwitterUser user)
        {
            _twitterUserID = user.id;
        }

        public IEnumerable<TwitterStatus> GetMessages()
        {
            throw new NotImplementedException();
        }
    }
}
