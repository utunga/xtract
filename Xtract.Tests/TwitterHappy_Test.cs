using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using NUnit.Framework;
using XtractLib.Net;
using XtractLib.OAuth;
using XtractLib.Twitter;

[TestFixture]
public class TwitterHappy_Test {

    [Test]
    public void ExercizeHappyFilter()
    {
        Assert.AreEqual(TwitterHappy.MIN_WAIT, TwitterHappy.RecommendedWait(), "Expect to start at 0");
        TwitterHappy.PrintStatus();
        TwitterHappy.SeemsGrumpy();
        TwitterHappy.PrintStatus();
        Assert.GreaterOrEqual(TwitterHappy.RecommendedWait(), 500, "Expect to increment to 500 on first failure");
        TwitterHappy.SeemsGrumpy();
        TwitterHappy.PrintStatus();
        TwitterHappy.SeemsGrumpy();
        TwitterHappy.PrintStatus();
        TwitterHappy.SeemsGrumpy();
        TwitterHappy.PrintStatus();
        TwitterHappy.SeemsHappy();
        TwitterHappy.PrintStatus();
        TwitterHappy.SeemsHappy();
        TwitterHappy.PrintStatus();
        TwitterHappy.SeemsHappy();
        TwitterHappy.PrintStatus();
        TwitterHappy.SeemsHappy();
        TwitterHappy.PrintStatus();
        TwitterHappy.SeemsHappy();
        Assert.GreaterOrEqual(TwitterHappy.RecommendedWait(), 0, "Expect to floor at 0 after a few rounds of everything good");

        bool caughtException = false;
        try
        {
            for (int i = 0; i < TwitterHappy.MAX_FAILED_ATTEMPTS + 1; i++)
            {
                TwitterHappy.SeemsGrumpy();
                TwitterHappy.PrintStatus();
            }
        }
        catch (ApplicationException)
        {
            caughtException = true;
        }

        Assert.That(caughtException, "Expected to encounter an ApplicationException after " + TwitterHappy.MAX_FAILED_ATTEMPTS + " calls to SeemsGrumpy()"); 


    }

   
}

