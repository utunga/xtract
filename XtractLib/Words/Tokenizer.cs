using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace XtractLib.Words
{
    public class Tokenizer
    {

        public static string URL_REGEX = @"\b(https?)://[-A-Z0-9+&@#/%?=~_|!:,.;]*[-A-Z0-9+&@#/%=~_|]"; // be sure to specify IgnoreCase when you use this
        private static Regex _urlMatch;
        private static Regex _hashOrAtMatch;
        private static Regex _wordsMatch;

        static Tokenizer()
        {
            //way too easy ;))
            //_urlMatch = new Regex(@"http\://\S+");
            _urlMatch = new Regex(URL_REGEX, RegexOptions.IgnoreCase);
            _hashOrAtMatch = new Regex(@"[#|@]\w+");
            _wordsMatch = new Regex(@"[\w|']+");
        }

        //Helper static method 
        public static string[] GetWords(string source)
        {
            IUrlExpander expander = new UrlExpander();
            Tokenizer tokenizer = new Tokenizer(expander);
            return new List<string>(tokenizer.Tokenize(source)).ToArray();
        }

        private IUrlExpander _expander;

        public Tokenizer(IUrlExpander expander)
        {
            _expander = expander;
        }

        public IEnumerable<string> Tokenize(string source)
        {
            source = source.Trim();
            IEnumerable<string> urls;
            List<string> result = ProcessIntoWordsAndUrls(source, out urls);
            result.AddRange(_expander.ProcessUrls(urls));
            return result;

        }

        private static List<string> ProcessIntoWordsAndUrls(string source, out IEnumerable<string> urls)
        {
            var words = new List<string>();
            var tmpUrls = new List<string>();
            if (_urlMatch.IsMatch(source))
            {
                string tmp = source;
                foreach (Match match in _urlMatch.Matches(source))
                {
                    //add to list of urls
                    tmpUrls.Add(match.Value);
                }
                //remove from source
                foreach (string url in tmpUrls)
                {
                    tmp = tmp.Replace(url, "");
                }
                source = tmp;
            }
           
            urls = tmpUrls; // assign out result


            //lower case hashtags, and preserve as uniq words
            if (_hashOrAtMatch.IsMatch(source))
            {
                string tmp = source;
                foreach (Match match in _hashOrAtMatch.Matches(source))
                {
                    string hashTag = match.Value;
                    
                    //remove from source
                    tmp = tmp.Replace(hashTag, "");

                    //add to results in lower invariant form
                    words.Add(hashTag.ToLowerInvariant());
                }
                source = tmp;
            }

            // translate words like "It's" to "Its" FIXME:need a smarter regex here
           // source = source.Replace("'", "");

            //lower case the first letter of otherwords, but otherwise             
            if (_wordsMatch.IsMatch(source))
            {
                foreach (Match match in _wordsMatch.Matches(source))
                {
                    string word = match.Value;
                    if (word.Length > 1)
                    {
                        words.Add(word.Substring(0, 1).ToUpperInvariant() + word.Substring(1));
                    }
                    else if (word.Length == 1)
                    {
                        words.Add(word.ToUpperInvariant());
                    }
                }
            }

            return words;
        }
    }
}