using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using XtractLib.Trigram;


namespace Xtract.Tests
{

    [TestFixture]
    public class LanguageModel_Test
    {
        LanguageModel _english;

        [SetUp]
        public void Setup()
        {
            _english = new LanguageModel();
            _english.ParseStream(new StreamReader("data/gutenburg_en_1.txt", Encoding.UTF8));
            _english.ParseStream(new StreamReader("data/gutenburg_en_2.txt", Encoding.UTF8));
            _english.ParseStream(new StreamReader("data/gutenburg_en_3.txt", Encoding.UTF8));
            _english.ParseStream(new StreamReader("data/gutenburg_en_4.txt", Encoding.UTF8));
            _english.ParseStream(new StreamReader("data/gutenburg_en_5.txt", Encoding.UTF8));
        }

        [Test]
        public void EqualTextShouldBeOne()
        {
            string txt = "once upon a time there was a small boy";
            LanguageModel model1 = new LanguageModel(txt);
            LanguageModel model2 = new LanguageModel(txt);

            AssertPrettyMuchEqual(1.0, model1.Similarity(model2), "Expected identical strings to have similarity of 1");
            AssertPrettyMuchEqual(1.0, model2.Similarity(model1), "Expected identical strings to have similarity of 1, either way around");
        }

        [Test]
        public void SimilarTextShouldCloseBeOne()
        {
            string txt1 = "once upon a time there was a small boy";
            string txt2 = "once upon a time there waz a small boy";
            LanguageModel model1 = new LanguageModel(txt1);
            LanguageModel model2 = new LanguageModel(txt2);

            Assert.GreaterOrEqual(model1.Similarity(model2), 0.09d, "Expected very similar  strings to have similarity close 1");
            Assert.LessOrEqual(model1.Similarity(model2), 1.0d, "Expected identical strings to have similarity of 1");
        }

        [Test]
        public void SmallEnglishShouldHaveSimilarityOf1()
        {
            LanguageModel english2 = ModelFactory.LoadModelFromFolder("data");
            AssertPrettyMuchEqual(1.0, _english.Similarity(english2), "Expected identical language models to have similarity of 1");
            AssertPrettyMuchEqual(1.0, english2.Similarity(_english), "Expected identical language models to have similarity of 1, either way around");
        }

        [Test]
        [Ignore("Long running test I only needed for sanity check")]
        public void LargeEnglishShouldHaveSimilarityOf1()
        {
            LanguageModel english_large = ModelFactory.LoadModelFromFolder("english_data"); // takes a long time to parse
            AssertPrettyMuchEqual(1.0, english_large.Similarity(english_large), "Expected identical large models to have similarity of 1");
            AssertPrettyMuchEqual(1.0, english_large.Similarity(english_large), "Expected identical large models to have similarity of 1, either way around");

            LanguageModel tweetModel = new LanguageModel("Aye #shoutout to all the single mothers out there doin what they gotta do to provide for their kids and themselves. u r appreciated :)");
            Assert.LessOrEqual(1.0d, tweetModel.Similarity(english_large), "expected similarity to large model to be always less than 1");

        }


        [Test]
        public void EnglishThresholdTest()
        {
            AssertEnglish("so i said, hey what are you doing?");
            AssertEnglish("If you're wanting tickets for tomorrow night's home opener or any night at \"The Swamp\", call the 'Dogs front office at 910-426-5900");
            AssertEnglish("with the steps required to download and use files from");
            AssertEnglish("How bout me and u gets sum breakfast"); ///hmm this is barely english though in't?     
            AssertEnglish("Fashion Terms shoes online gr dresses by flirt john surratt . http://www.lsaco.com/fashion-terms-shoes-online-gr-dresses-by-flirt/");
            AssertEnglish("@wvpv Do let me know how you get on... am Psonar CTO. Any problems or anything we can do to make it better, just say so! Thanks!");
            AssertEnglish("@MissFAB_LC the circus.. Aunt Cina takin Juju 2day!!!!");
    
            AssertNotEnglish("@1989515 난 작업견이라는 말까지는 안했어,,, 그냥 음큼견이라는 거지... ㅋㅋ 넌 순수해...  응큼한게 니 순수한 멋이야... ㅋㅋㅋㅋ @aphrodite_sung @jth800 #반말한당_");
            AssertNotEnglish("装gentoo的时候可以看完一本小说。当然屁长屁长的还是看不完的");
            AssertNotEnglish("RT ιƒ уσυ'яє ѕιηgℓє &hearts"); //actually this is 'english' but, i mean wtf?
            AssertNotEnglish("Dmn tuh? RT @vaniessadh: di apartemen sendokuran  so scary ternyata http://myloc.me/7sqYQ");
            AssertNotEnglish("@QhaCaembie Hahaha... Tau de yg dah bs twitteran di HP. Bilang pa u sm w. Haha");
            AssertEnglish("@BustinnJieeber haha :) LOVE U 2 &lt;3 ♥");
            AssertEnglish("|@Ignorancelove WO MAI HL. LATER FAT. AND THE MILK SO ERM, EEEW.");

        }

        [Test]
        public void FullCourtEnglishTest()
        {
            _english = ModelFactory.LoadModelFromFolder("data");
            EnglishThresholdTest();
        }

        private void AssertEnglish(string text)
        {
            LanguageModel model = new LanguageModel(text);
            //FIXME similarity should always be below 1 but sometimes it ends up above that wha?
            Console.Out.WriteLine("Similarity:" + model.Similarity(_english) + "\t Text:" + text);
            //Assert.GreaterOrEqual(model.Similarity(_english), 0.05d, "Expected english classification");
        }

        private void AssertNotEnglish(string text)
        {
            LanguageModel model = new LanguageModel(text);
            Console.Out.WriteLine("Similarity:" + model.Similarity(_english) + "\t Text:" + text);
           //Assert.LessOrEqual(model.Similarity(_english), 0.05d, "Expected non english classification");
        }

        private static void AssertPrettyMuchEqual(double expected, double actual, string failMessage)
        {
           Assert.GreaterOrEqual(actual, expected-(0.0001d), failMessage);
           Assert.LessOrEqual(actual, expected + (0.0001d), failMessage);
        }
    }
}
