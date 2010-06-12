using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace XtractLib.OAuth
{
    public static class TwitterAuth
    {
        const string AUTH_KEY = "oauth";
        const string USER_KEY = "oauth_user";

        public static bool HasValidSession()
        {
            return ((HttpContext.Current != null) &&
                    (HttpContext.Current.Session[AUTH_KEY] != null) &&
                    (HttpContext.Current.Session[USER_KEY] != null));
        }

        public static OAuthTwitter CurrentSession
        {
            get 
            {
                if ((HttpContext.Current != null) &&
                    (HttpContext.Current.Session[AUTH_KEY] != null))
                {
                    return (OAuthTwitter)HttpContext.Current.Session[AUTH_KEY];
                }
                return null;
            }
        }


    }
}