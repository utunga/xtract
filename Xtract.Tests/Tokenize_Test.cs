using NUnit.Framework;
using XtractLib.Common;

[TestFixture]
public class Tokenize_Test
{
    // Methods
    [Test]
    public void WordsTest()
    {
        string[] result = Tokenize.Words("|#whatNOTtosayaftersex :: when is u leavin. U been here for a good hour now.");
        Assert.Contains("leavin", result, "Expect basic word split single word");
    }

    [Test]
    public void WordsTestHash()
    {
        string[] result = Tokenize.Words("|#whatNOTtosayaftersex :: when is u leavin. U been here for a good hour now.");
        Assert.Contains("#whatNOTtosayaftersex", result, "Expect hash tags to be a single word");
    }

    [Test]
    public void WordsTestLower()
    {
        string[] result = Tokenize.Words("|#whatNOTtosayaftersex :: When is u leavin. U been here for a good hour now.");
        Assert.Contains("when", result, "Expect words to be all lower case");
    }

    [Test]
    public void WordsTestURISolid()
    {
        string[] result = Tokenize.Words("No matter how clever you are, it's going to be http://twittascope.com/?sign=3 to av... More for Gemini ");
        Assert.Contains("http://twittascope.com/?sign=3", result, "Expect urls to be maintained intact");
        result = Tokenize.Words("No matter how clever you are, it's going to be difficult to av... More for Gemini http://twittascope.com/?sign=3");
        Assert.Contains("http://twittascope.com/?sign=3", result, "Expect urls to be self contained");
    }
}

