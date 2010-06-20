namespace XtractLib.Net
{
    public interface IResponseBuilder 
    {
         
        /// <summary>
        /// Submit a GET web request using oAuth.
        /// </summary>
        /// <param name="url">The full url, including the querystring.</param>
        /// <returns>The web server response.</returns>
        IResponseReader GetResponseReader(string url);

        /// <summary>
        /// Submit a web request using oAuth.
        /// </summary>
        /// <param name="method">GET or POST</param>
        /// <param name="url">The full url, including the querystring.</param>
        /// <param name="postData">Data to post (querystring format)</param>
        /// <returns>The web server response.</returns>
        IResponseReader GetResponseReader(WebMethod method, string url, string postData);
    }
}