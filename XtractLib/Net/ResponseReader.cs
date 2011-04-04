using System;
using System.IO;
using System.Net;
using XtractLib.OAuth;

namespace XtractLib.Net
{
    /// <summary>
    /// The wheel, reinvented. Encapsulates all stream response crap and this and that
    /// </summary>
    public class ResponseReader : IDisposable, IResponseReader
    {
        private readonly ICredentials _credentials;

        private readonly WebMethod _method;
        private readonly string _url;
        private readonly string _postData;

        private StreamReader _reader;
        private HttpWebRequest _request;
        private WebResponse _response;

        public ResponseReader(WebMethod method, string url, string postData)
        {
            _method = method;
            _url = url;
            _postData = postData;
        }

        public ResponseReader(WebMethod method, string url, string postData, ICredentials credentials)
            : this(method, url, postData)
        {
            _credentials = credentials;
        }

        public string ReadLine()
        {

            EnsureReader();
            return _reader.ReadLine();

        }

        public string ReadToEnd()
        {
            string results;
            try
            {
                EnsureReader();
                results = _reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                results = "";
            }
            finally 
            {
                 CloseReader();
            }
            return results;
        }

        private void EnsureReader()
        {
            if (_reader==null)
            {
                _request = WebRequest.Create(_url) as HttpWebRequest;
                _request.Method = _method.ToString();
                _request.ServicePoint.Expect100Continue = false;
                _request.UserAgent = "SocialMapVerse";
                _request.Timeout = 10000;
                if (_credentials != null)
                {
                    _request.Credentials = _credentials;
                }

                if (_method == WebMethod.POST)
                {
                    _request.ContentType = "application/x-www-form-urlencoded";

                    //POST the data.
                    StreamWriter requestWriter = new StreamWriter(_request.GetRequestStream());
                    try
                    {
                        requestWriter.Write(_postData);
                    }
                    finally
                    {
                        requestWriter.Close();
                    }
                }
              
                _response = _request.GetResponse();
                _reader = new StreamReader(_response.GetResponseStream());
            } 
        }

        private void CloseReader()
        {
            //try
            //{
                if (_request != null) _request.Abort();
                if (_response != null) _response.Close();
                if (_reader != null) _reader.Close();
            //}
            //catch (Exception ex) {
            //    Console.Out.WriteLine("Exception occured while closing reader:" + ex.Message + "\n\n" + ex.StackTrace);
            //}
        }

        public void Dispose()
        {
            CloseReader();
        }

    }
}