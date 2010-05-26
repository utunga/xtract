using System;
using NUnit.Framework;
using XtractLib.Twitter;

[TestFixture]
public class EnglishStatusProvider_Test
{
    private EnglishStatusProvider _target;
   
    [SetUp]
    public void Setup()
    {
        TwitterStatusProvider twitterStatusProvider = new TwitterStatusProvider();
        twitterStatusProvider.UseCGICredentials("utunga", "a1ma4a5");
        twitterStatusProvider.YieldThisMany = 5;
        _target = new EnglishStatusProvider(twitterStatusProvider);
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

