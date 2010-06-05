using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XtractLib.Twitter;

namespace XtractLib.Words
{
    public class MessageProcessor : IMessageReceiver<TwitterStatus>
    {
        private Tokenizer _tokenizer;
        MessageProcessor(Tokenizer tokenizer)
        {
            _tokenizer = tokenizer;
        }

        public void Notify(TwitterStatus status)
        {
            string username = status.user.name;
            foreach(string word in _tokenizer.Tokenize(status.text))
            {
                Console.Out.WriteLine(username + "|" + word);
            }
        }
    }
}
