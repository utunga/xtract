using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using NUnit.Framework;
using XtractLib.Net;
using XtractLib.OAuth;
using XtractLib.Twitter;

[TestFixture]
public class RateLimitCheck_Test
{
    IResponseBuilder _oAuthTwitter;

    // Methods
    [SetUp]
    public void Setup()
    {
        //string twitter_api_username = ConfigurationManager.AppSettings["twitter_user"];
        //string twitter_api_password = ConfigurationManager.AppSettings["twitter_pass"];
        //_credentials = new NetworkCredential(twitter_api_username, twitter_api_password);
        _oAuthTwitter = new OAuthTwitterResponseBuilder();
    }

    [Test]
    public void TestGetStatus()
    {
        TwitterRateLimitStatus status = RateLimitCheck.GetStatus(_oAuthTwitter);
        Assert.LessOrEqual(status.remaining_hits, status.hourly_limit, "rate check bit nuts");
    }

    [Test]
    public void TestWaitTillOK()
    {
        //not really sure how to test this
        RateLimitCheck.SleepTillOKToProceed(_oAuthTwitter);
    }
}

