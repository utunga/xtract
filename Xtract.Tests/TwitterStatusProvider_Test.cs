using System;
using System.Collections.Generic;
using NUnit.Framework;
using XtractLib.Twitter;

[TestFixture]
public class TwitterStatusProvider_Test
{
    // Fields
    private TwitterStreamStatusProvider _target;

    // Methods
    [SetUp]
    public void Setup()
    {
        _target = new TwitterStreamStatusProvider();
        _target.UseCGICredentials("utunga", "a1ma4a5");
        _target.YieldThisMany = 10;
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

