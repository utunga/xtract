using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using XtractLib.Net;

namespace XtractLib.Net
{
    public class WebResponseBuilder : IResponseBuilder
    {

        ICredentials _credentials;

        public void UseCGICredentials(string twitter_api_username, string twitter_api_password)
        {
            _credentials = new NetworkCredential(twitter_api_username, twitter_api_password);
        }

        /// <summary>
        /// Submit a GET web request using oAuth.
        /// </summary>
        /// <param name="url">The full url, including the querystring.</param>
        /// <returns>The web server response.</returns>
        public IResponseReader GetResponseReader(string url)
        {
            return GetResponseReader(WebMethod.GET, url, string.Empty);
        }

        /// <summary>
        /// Submit a web request using oAuth.
        /// </summary>
        /// <param name="method">GET or POST</param>
        /// <param name="url">The full url, including the querystring.</param>
        /// <param name="postData">Data to post (querystring format)</param>
        /// <returns>The web server response.</returns>
        public IResponseReader GetResponseReader(WebMethod method, string url, string postData)
        {
            string querystring = "";

            //Setup postData for signing.
            //Add the postData to the querystring.
            if (method == WebMethod.POST)
            {
                if (postData.Length > 0)
                {
                    //Decode the parameters and re-encode using the oAuth UrlEncode method.
                    NameValueCollection qs = HttpUtility.ParseQueryString(postData);
                    postData = "";
                    foreach (string key in qs.AllKeys)
                    {
                        if (postData.Length > 0)
                        {
                            postData += "&";
                        }
                        qs[key] = HttpUtility.UrlDecode(qs[key]);
                        qs[key] = HttpUtility.UrlEncode(qs[key]);
                        postData += key + "=" + qs[key];

                    }
                    if (url.IndexOf("?") > 0)
                    {
                        url += "&";
                    }
                    else
                    {
                        url += "?";
                    }
                    url += postData;
                }
            }

            Uri uri = new Uri(url);
            
            //Convert the querystring to postData
            if (method == WebMethod.POST)
            {
                postData = querystring;
                querystring = "";
            }
            
            string finalUrl = uri + ((querystring.Length > 0)?  "?" + querystring: "");
            return new ResponseReader(method, finalUrl, postData, _credentials);
        }


        ///// <summary>
        ///// Process the web response.
        ///// </summary>
        ///// <param name="webRequest">The request object.</param>
        ///// <returns>The response data.</returns>
        //public string WebResponseGet(HttpWebRequest webRequest)
        //{
        //    StreamReader responseReader = null;
        //    string responseData = "";

        //    try
        //    {
        //        responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
        //        responseData = responseReader.ReadToEnd();
        //    }
        //    finally
        //    {
        //        webRequest.GetResponse().GetResponseStream().Close();
        //        if (responseReader != null) responseReader.Close();
        //    }
        //    return responseData;
        //}
        
    }
}