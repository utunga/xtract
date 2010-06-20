using System;
using System.Configuration;
using NUnit.Framework;
using XtractLib.Net;
using XtractLib.Twitter;

[TestFixture]
public class EnglishStatusProvider_Test
{
    private EnglishStatusProvider _target;
   
    [SetUp]
    public void Setup()
    {
        WebResponseBuilder responseBuilder = new WebResponseBuilder();
        string twitter_api_username = ConfigurationManager.AppSettings["twitter_user"];
        string twitter_api_password = ConfigurationManager.AppSettings["twitter_pass"];
        responseBuilder.UseCGICredentials(twitter_api_username, twitter_api_password);

        TwitterStreamStatusProvider twitterStatusProvider = new TwitterStreamStatusProvider(responseBuilder);
         twitterStatusProvider.YieldThisMany = 5;
        _target = new EnglishStatusProvider(twitterStatusProvider, "data");
        _target.Threshold = 0.05d;
    }

    [Test]
    public void TestReceive()
    {
        foreach (TwitterStatus status in _target.GetMessages())
        {
            Console.Out.WriteLine(string.Concat(new object[] { status.user.id, "|", status.user.screen_name, "|", status.text }));
        }
    }
}

