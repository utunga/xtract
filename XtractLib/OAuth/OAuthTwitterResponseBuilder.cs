using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using XtractLib.Net;

namespace XtractLib.OAuth
{
    public class OAuthTwitterResponseBuilder : OAuthBase, IResponseBuilder
    {
       
        public const string REQUEST_TOKEN = "http://api.twitter.com/oauth/request_token";
        public const string AUTHORIZE = "http://api.twitter.com/oauth/authorize";
        public const string ACCESS_TOKEN = "http://api.twitter.com/oauth/access_token";

        private string _consumerKey = "";
        private string _consumerSecret = "";
        private string _token = "";
        private string _tokenSecret = "";
        private string _oAuthVerifier = "";
        private string _callBackUrl = "";

        #region Properties
        public string ConsumerKey 
        {
            get
            {
                if (_consumerKey.Length == 0)
                {
                    _consumerKey = ConfigurationManager.AppSettings["consumerKey"] ?? "";
                }
                return _consumerKey; 
            } 
            set { _consumerKey = value; } 
        }
        
        public string ConsumerSecret { 
            get {
                if (_consumerSecret.Length == 0)
                {
                    _consumerSecret = ConfigurationManager.AppSettings["consumerSecret"] ?? "";
                }
                return _consumerSecret; 
            }
            set { _consumerSecret = value; } 
        }

        public string Token
        {
            get
            {
                if (_token.Length == 0)
                {
                    _token = ConfigurationManager.AppSettings["oAuthToken"] ?? "";
                }
                return _token;
            }
            set { _token = value; }
        }

        public string OAuthVerifier
        {
            get
            {
                if (_oAuthVerifier.Length == 0)
                {
                    _oAuthVerifier = ConfigurationManager.AppSettings["oAuthVerifier"] ?? "";
                }
                return _oAuthVerifier;
            }
            set { _oAuthVerifier = value; }
        }

        public string TokenSecret
        {
            get
            {
                if (_tokenSecret.Length == 0)
                {
                    _tokenSecret = ConfigurationManager.AppSettings["tokenSecret"] ?? "";
                }
                return _tokenSecret;
            }
            set { _tokenSecret = value; }
        }

        public string CallBackUrl { get { return _callBackUrl; } set { _callBackUrl = value; } }

        #endregion

        /// <summary>
        /// Get the link to Twitter's authorization page for this application.
        /// </summary>
        /// <returns>The url with a valid request token, or a null string.</returns>
        public string AuthorizationLinkGet(out string oAuthToken)
        {
            string ret = null;

            IResponseReader reader = GetResponseReader(WebMethod.GET, REQUEST_TOKEN, String.Empty);
            oAuthToken = "ERROR OCCURED NO oAuthToken in response";
            string response = reader.ReadToEnd();
            if (response.Length > 0)
            {
                //response contains token and token secret.  We only need the token.
                NameValueCollection qs = HttpUtility.ParseQueryString(response);
                if (qs["oauth_token"] != null)
                {
                    oAuthToken = qs["oauth_token"];
                    ret = AUTHORIZE + "?oauth_token=" + oAuthToken;
                }
            }
            return ret;
        }

        /// <summary>
        /// Exchange the request token for an access token.
        /// </summary>
        /// <param name="authToken">The oauth_token is supplied by Twitter's authorization page following the callback.</param>
        public void AccessTokenGet(string authToken)
        {
            this.Token = authToken;

            IResponseReader reader = GetResponseReader(WebMethod.GET, ACCESS_TOKEN, String.Empty);
            string response = reader.ReadToEnd();

            if (response.Length > 0)
            {
                //Store the Token and Token Secret
                NameValueCollection qs = HttpUtility.ParseQueryString(response);
                if (qs["oauth_token"] != null)
                {
                    this.Token = qs["oauth_token"];
                }
                if (qs["oauth_token_secret"] != null)
                {
                    this.TokenSecret = qs["oauth_token_secret"];
                }
            }
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
            string outUrl = "";
            string querystring = "";
            string ret = "";

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
                        qs[key] = this.UrlEncode(qs[key]);
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
            
            string nonce = this.GenerateNonce();
            string timeStamp = this.GenerateTimeStamp();

            //Generate Signature
            string sig = this.GenerateSignature(uri,
                                                this.ConsumerKey,
                                                this.ConsumerSecret,
                                                this.Token,
                                                this.TokenSecret,
                                                this.CallBackUrl,
                                                this.OAuthVerifier,
                                                method.ToString(),
                                                timeStamp,
                                                nonce,
                                                out outUrl,
                                                out querystring);

            querystring += "&oauth_signature=" + UrlEncode(sig);

            //Convert the querystring to postData
            if (method == WebMethod.POST)
            {
                postData = querystring;
                querystring = "";
            }

            if (querystring.Length > 0)
            {
                outUrl += "?";
            }

            return new ResponseReader(method, outUrl +  querystring, postData);
        }

        public string GetResponse(WebMethod method, string url, string postData)
        {
            IResponseReader responseReader = GetResponseReader(method, url, postData);
            return responseReader.ReadToEnd();
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