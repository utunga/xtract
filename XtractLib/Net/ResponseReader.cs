using System;
using System.IO;
using System.Net;

namespace XtractLib.Net
{
    /// <summary>
    /// The wheel, reinvented. Encapsulates all stream response crap and this and that
    /// </summary>
    public class ResponseReader : IDisposable, IResponseReader
    {
        private readonly ICredentials _credentials;
        private readonly string _uri;

        private StreamReader _reader;
        private HttpWebRequest _request;
        private WebResponse _response;

        public ResponseReader(string uri, ICredentials credentials)
        {
            _credentials = credentials;
            _uri = uri;
        }

        public string ReadLine()
        {
            EnsureReader();
            return _reader.ReadLine();
        }

        private void EnsureReader()
        {
            if (_reader==null)
            {
                _request = WebRequest.Create(_uri) as HttpWebRequest;
                _request.ServicePoint.Expect100Continue = false;
                _request.UserAgent = "TwadeMe";
                _request.Timeout = 10000;
                if (_credentials != null)
                {
                    _request.Credentials = _credentials;
                }
                _response = _request.GetResponse();
                _reader = new StreamReader(_response.GetResponseStream());
            } 
        }

        private void CloseReader()
        {
            if (_request != null) _request.Abort();
            if (_response != null) _response.Close();
            if (_reader != null) _reader.Close();
        }

        public void Dispose()
        {
            CloseReader();
        }
    }
}