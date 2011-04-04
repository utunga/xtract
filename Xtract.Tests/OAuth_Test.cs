

using System;
using System.Collections.Specialized;
using System.Web;
using NUnit.Framework;
using XtractLib.Net;
using XtractLib.OAuth;
using XtractLib.Twitter;

namespace Xtract.Tests
{

    [TestFixture]
    public class OAuth_Test
    {
        [Test]
        public void OAuthCheckRateLimit()
        {
            OAuthTwitterResponseBuilder twitterAuth = new OAuthTwitterResponseBuilder();
            TwitterRateLimitStatus status = RateLimitCheck.GetStatus(twitterAuth);
            Console.Out.WriteLine(" remaining_hits:" + status.remaining_hits + " hourly_limit:" + status.hourly_limit + " reset_time:" + status.reset_time + " reset_time_in_seconds:" + status.reset_time_in_seconds);
            Assert.LessOrEqual(status.remaining_hits, status.hourly_limit, "rate check bit nuts");
            Assert.GreaterOrEqual(status.hourly_limit, 350, "expect to have 350 now");
            
        }

        [Test]
        public void OAuthCheck()
        {
            OAuthTwitterResponseBuilder twitterAuth = new OAuthTwitterResponseBuilder();
         
            string url = "http://twitter.com/account/verify_credentials.xml";
            string xml = twitterAuth.GetResponse(WebMethod.GET, url, String.Empty);
            Console.Out.WriteLine(xml);
        }


        [Test]
        [Ignore("really just here as a debugging exercize, dont use")]
        public void OAuthAuthorizeSequence()
        {
            
            string REQUEST_TOKEN = "http://api.twitter.com/oauth/request_token";
            string AUTHORIZE = "http://api.twitter.com/oauth/authorize";
            string ACCESS_TOKEN = "http://api.twitter.com/oauth/access_token";

            //1. The application uses oauth/request_token to obtain a request token from twitter.com.
        
            OAuthTwitterResponseBuilder twitterAuth = new OAuthTwitterResponseBuilder();
            //Redirect the user to Twitter for authorization.
            //Using oauth_callback for local testing.
            string oAuthToken;
            string response = twitterAuth.GetResponse(WebMethod.GET, REQUEST_TOKEN, String.Empty);
            oAuthToken = "ERROR OCCURED NO oAuthToken in response";
            if (response.Length > 0)
            {
                //response contains token and token secret.  We only need the token.
                NameValueCollection qs = HttpUtility.ParseQueryString(response);
                if (qs["oauth_token"] != null)
                {
                    oAuthToken = qs["oauth_token"];
                }
            }

            //2. The application directs the user to oauth/authorize on twitter.com.
            string authURL= AUTHORIZE + "?oauth_token=" + oAuthToken;
            Console.Out.WriteLine("Please visit following url to authorize:");
            Console.Out.WriteLine(authURL);

            Console.ReadKey();

          
            //3. After obtaining approval from the user, a prompt on twitter.com will display a 7 digit PIN.
            //4. The user is instructed to copy this PIN and return to the appliction.
            //5. The application will prompt the user to enter the PIN from step 4.
            string PIN = Console.ReadLine();

            //6. The application uses the PIN as the value for the oauth_verifier parameter in a call to oauth/access_token which will verify the PIN and exchange a request_token for an access_token.
            twitterAuth.Token = oAuthToken;
            twitterAuth.OAuthVerifier = PIN;
            response = twitterAuth.GetResponse(WebMethod.GET, ACCESS_TOKEN, String.Empty);

            //7. Twitter will return an access_token for the application to generate subsequent OAuth signatures.
            if (response.Length > 0)
            {
                //Store the Token and Token Secret
                NameValueCollection qs = HttpUtility.ParseQueryString(response);
                if (qs["oauth_token"] != null)
                {
                    twitterAuth.Token = qs["oauth_token"];
                }
                if (qs["oauth_token_secret"] != null)
                {
                    twitterAuth.TokenSecret = qs["oauth_token_secret"];
                }
            }

            Console.Out.WriteLine("Old key :'" + oAuthToken + "'");
            Console.Out.WriteLine("New? key :'" + twitterAuth.Token + "'");
            Console.Out.WriteLine("TokenSecret  :'" + twitterAuth.TokenSecret + "'");
            Console.Out.WriteLine("Checking it worked");

            string url = "http://twitter.com/account/verify_credentials.xml";
            string xml = twitterAuth.GetResponse(WebMethod.GET, url, String.Empty);

            Console.Out.WriteLine(xml);
            
          
            Console.Out.WriteLine("Checking it worked again");
            xml = twitterAuth.GetResponse(WebMethod.GET, url, String.Empty);

            Console.Out.WriteLine(xml);
            Console.ReadKey();

        }


        ///// <summary>
        ///// Executes a shell command synchronously.
        ///// </summary>
        ///// <param name="command">string command</param>
        ///// <returns>string, as output of the command.</returns>
        //public void ExecuteCommandSync(string command)
        //{
        //    try
        //    {
        //        // create the ProcessStartInfo using "cmd" as the program to be run,
        //        // and "/c " as the parameters.
        //        // Incidentally, /c tells cmd that we want it to execute the command that follows,
        //        // and then exit.
        //        System.Diagnostics.ProcessStartInfo procStartInfo =
        //            new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);

        //        // The following commands are needed to redirect the standard output.
        //        // This means that it will be redirected to the Process.StandardOutput StreamReader.
        //        procStartInfo.RedirectStandardOutput = true;
        //        procStartInfo.UseShellExecute = false;
        //        // Do not create the black window.
        //        procStartInfo.CreateNoWindow = true;
        //        // Now we create a process, assign its ProcessStartInfo and start it
        //        System.Diagnostics.Process proc = new System.Diagnostics.Process();
        //        proc.StartInfo = procStartInfo;
        //        proc.Start();
        //        // Get the output into a string
        //        string result = proc.StandardOutput.ReadToEnd();
        //        // Display the command output.
        //        Console.WriteLine(result);
        //    }
        //    catch (Exception objException)
        //    {
        //        // Log the exception
        //    }
        //}


    }




}