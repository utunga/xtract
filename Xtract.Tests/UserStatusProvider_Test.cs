using System;
using System.Collections.Generic;
using System.Configuration;
using NUnit.Framework;
using XtractLib.Twitter;

[TestFixture]
public class UserStatusProvider_Test
{
    // Fields
    private UserStatusProvider _target;

    // Methods
    [SetUp]
    public void Setup()
    {

        _target = new UserStatusProvider("bnolan");
        string twitter_api_username = ConfigurationManager.AppSettings["twitter_user"];
        string twitter_api_password = ConfigurationManager.AppSettings["twitter_pass"];
        _target.UseCGICredentials(twitter_api_username, twitter_api_password);
        //_target.YieldThisMany = 10;
    }

    [Test]
    public void TestReceive()
    {
        List<TwitterStatus> received = new List<TwitterStatus>();
        foreach (TwitterStatus status in _target.GetMessages())
        {
            string text = status.text.Replace("|", " ");
            Console.Out.WriteLine(string.Concat(new object[] { status.user.id, "|", status.user.screen_name, "|", text }));
            received.Add(status);
        }
        Assert.Greater(received.Count, 5, "Expected at least 5 messages");
    }
}

