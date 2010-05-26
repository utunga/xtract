

using System.Collections.Generic;

namespace XtractLib.Twitter
{
    public class EnglishStatusProvider : IMessageProvider<TwitterStatus>
    {
        IMessageProvider<TwitterStatus> _statusProvider;

        public EnglishStatusProvider(IMessageProvider<TwitterStatus> statusProvider)
        {
            _statusProvider = statusProvider;
        }

        public IEnumerable<TwitterStatus> GetMessages()
        {
            foreach (TwitterStatus status in _statusProvider.GetMessages())
            {
                if (status.user != null && status.user.lang == "en")
                {
                    yield return status;
                }
            }
        }

    }
}
