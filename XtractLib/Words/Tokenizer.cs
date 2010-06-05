using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace XtractLib.Words
{
    public class Tokenizer
    {

        public static string URL_REGEX = @"\b(https?)://[-A-Z0-9+&@#/%?=~_|!:,.;]*[-A-Z0-9+&@#/%=~_|]"; // be sure to specify IgnoreCase when you use this
        private static Regex _urlMatch;
        private static Regex _hashMatch;
        private static Regex _wordSplit;

        static Tokenizer()
        {
            //way too easy ;))
            //_urlMatch = new Regex(@"http\://\S+");
            _urlMatch = new Regex(URL_REGEX, RegexOptions.IgnoreCase);
            _hashMatch = new Regex(@"#\w+");
            _wordSplit = new Regex(@"\W+");
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
            IEnumerable<string> urls;
            List<string> result = ProcessIntoWordsAndUrls(source, out urls);
            //TODO: process urls
            result.AddRange(_expander.ProcessUrls(urls));
            return result;

        }

        private static List<string> ProcessIntoWordsAndUrls(string source, out IEnumerable<string> urls)
        {
            var words = new List<string>();
            var tmpUrls = new List<string>();
            if (_urlMatch.IsMatch(source))
            {
                foreach (Match match in _urlMatch.Matches(source))
                {
                    //add to list of urls
                    tmpUrls.Add(match.Value);
                }
                //remove from source
                foreach (string url in words)
                {
                    source.Replace(url, "");
                }
            }
            urls = tmpUrls; // assign out result

            //lower case hashtags, and preserve as uniq words
            if (_hashMatch.IsMatch(source))
            {
                string tmp = source;
                foreach (Match match in _hashMatch.Matches(source))
                {
                    string hashTag = match.Value;
                    
                    //remove from source
                    tmp = tmp.Replace(hashTag, "");

                    //add to results in lower invariant form
                    words.Add(hashTag.ToLowerInvariant());
                }
                source = tmp;
            }

            //lower case the first letter of otherwords, but otherwise             
            foreach(string word in _wordSplit.Split(source))
            {
                if (word.Length > 1)
                {
                    words.Add(word.Substring(0, 1).ToUpperInvariant() + word.Substring(1));
                }
                else
                {
                    words.Add(word.ToUpperInvariant());
                }
            }

            return words;
        }
    }
}