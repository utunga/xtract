using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace XtractLib.Words
{
    /// <summary>
    /// Pulls messages from provided MessageProvider, and writes them to MessageReceiver, on different thread
    /// 
    /// TODO: Implement actual multi threading
    /// </summary>
    public class MessageBus<T>
    {
        private IMessageProvider<T> _provider;
        private IMessageReceiver<T> _receiver;
        private bool _cancel;
        public MessageBus(IMessageProvider<T> provider, IMessageReceiver<T> receiver)
        {
            _provider = provider;
            _receiver = receiver;
            _cancel = false;
        }

        public void Start()
        {
            while(!_cancel)
            {
                foreach (var message in _provider.GetMessages())
                {
                    _receiver.Notify(message);
                    if (_cancel) break;
                }
            }
            _cancel = false; //so 'start' can be called again
        }

        public void Cancel()
        {
            _cancel = true;
        }
    }
}
