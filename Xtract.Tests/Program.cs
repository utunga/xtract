using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XtractLib.Trigram;

namespace Xtract.Tests
{
    public class Program
    {
        private static void Main(string[] args)
        {
            OAuthTest();
        }

        private static void OAuthTest()
        {
            OAuth_Test test = new OAuth_Test();
            test.OAuthSequence();
        }

        private static void SimilarityTest()
        {
            List<string> examples = new List<string>() {
                "so i said, hey what are you doing?",
                "If you're wanting tickets for tomorrow night's home opener or any night at \"The Swamp\", call the 'Dogs front office at 910-426-5900",
                "with the steps required to download and use files from",
                "How bout me and u gets sum breakfast", ///hmm this is barely english though in't?     
                "Fashion Terms shoes online gr dresses by flirt john surratt . http://www.lsaco.com/fashion-terms-shoes-online-gr-dresses-by-flirt/",
                "@wvpv Do let me know how you get on... am Psonar CTO. Any problems or anything we can do to make it better, just say so! Thanks!",
                "@MissFAB_LC the circus.. Aunt Cina takin Juju 2day!!!!",
                "@1989515 난 작업견이라는 말까지는 안했어,,, 그냥 음큼견이라는 거지... ㅋㅋ 넌 순수해...  응큼한게 니 순수한 멋이야... ㅋㅋㅋㅋ @aphrodite_sung @jth800 #반말한당_",
                "装gentoo的时候可以看完一本小说。当然屁长屁长的还是看不完的",
                "RT ιƒ уσυ'яє ѕιηgℓє &hearts", //actually this is 'english' but, i mean wtf?
                "Dmn tuh? RT @vaniessadh: di apartemen sendokuran  so scary ternyata http://myloc.me/7sqYQ",
                "@QhaCaembie Hahaha... Tau de yg dah bs twitteran di HP. Bilang pa u sm w. Haha",
                "@BustinnJieeber haha :) LOVE U 2 &lt;3 ♥",
                "|@Ignorancelove WO MAI HL. LATER FAT. AND THE MILK SO ERM, EEEW." };
            LanguageModel english = ModelFactory.LoadModelFromFolder("data");

            SortedList<double, string> classify = new SortedList<double, string>();
            foreach (string example in examples)
            {
                LanguageModel test = new LanguageModel(example);
                classify.Add(test.Similarity(english), example);
            }

            foreach (var example in classify)
            {
                Console.Out.WriteLine(example.Key + "|" + example.Value);
            }
        }
    }
}
