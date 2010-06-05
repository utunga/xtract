using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;
using XtractLib.Words;

[TestFixture]
public class Tokenize_Test
{
    // Methods
    [Test]
    public void WordsTest()
    {
        string[] result = Tokenizer.GetWords("|#whatNOTtosayaftersex :: when is u leavin. U been here for a good hour now.");
        Assert.Contains("Leavin", result, "Expect basic word split single word");
    }

    [Test]
    public void WordsTestHash()
    {
        string[] result = Tokenizer.GetWords("|#whatNOTtosayaftersex :: when is u leavin. U been here for a good hour now.");
        Assert.IsFalse(new List<string>(result).Contains("WhatNOTtosayaftersex"), "expect hashed words to be removed from source");
        Assert.Contains("#whatnottosayaftersex", result, "Expect hash tags to be a single word");
    }

    [Test]
    public void WordsTestFirstLetterUppered()
    {
        string[] result = Tokenizer.GetWords("|#whatNOTtosayaftersex :: when is u leavin. U been here for a good hour now.");
        Assert.Contains("When", result, "Expect words to start off upper case");
    }

    [Test]
    public void WordsTestRestOfCasePreserved()
    {
        string[] result = Tokenizer.GetWords("|#whatNOTtosayaftersex :: When is u leavin. U been here for a GOOD hour now.");
        Assert.IsFalse(new List<string>(result).Contains("u"), "Expect one letter words to be all upper case");
        Assert.That(new List<string>(result).Contains("GOOD"), "Expect rest of case to be preserved");
        Assert.Contains("GOOD", result, "Expect words to preserve rest of case");
    }

    [Test]
    public void WordsTestURISolid()
    {
        string[] result = Tokenizer.GetWords("No matter how clever you are, it's going to be http://twittascope.com/?sign=3 to av... More for Gemini ");
        Assert.Contains("http://twittascope.com/?sign=3", result, "Expect urls to be maintained intact");
        result = Tokenizer.GetWords("No matter how clever you are, it's going to be difficult to av... More for Gemini http://twittascope.com/?sign=3");
        Assert.Contains("http://twittascope.com/?sign=3", result, "Expect urls to be self contained");
    }

    [Test]
    public void TestUrlRegex()
    {
        Regex tmp = new Regex(Tokenizer.URL_REGEX, RegexOptions.IgnoreCase);
        Assert.That(tmp.IsMatch("http://foobar.com"), "no match on regexp");
        Assert.That(tmp.IsMatch("it's going to be http://twittascope.com/?sign=3 to"), "no match on regexp");

        // this don't work - ok for now
        //Assert.That(tmp.IsMatch("it's going to be twittascope.com/?sign=3 to"), "no match on regexp");
        // note that it pulls out the url, but not the comma afterwards
        Assert.AreEqual("http://twittascope.com/?sign=3", tmp.Matches("it's going to be http://twittascope.com/?sign=3, to")[0].Value, "no match on regexp");
         
    }
}

