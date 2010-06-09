using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace XtractLib.Trigram
{
    /// <summary>
    /// Models a 'language' by counting occurences of trigrams in the supplied stream (including space as just another character)
    /// 
    /// Algorithm for language similarity courtesy of Douglas Bagnell's python recipe found here:
    /// http://code.activestate.com/recipes/326576/ - thanks Douglas! 
    /// </summary>
    public class LanguageModel 
    {
        private double? _length;
        private readonly SortedList<char, SortedList<char, SortedList<char, long>>> _trigramModel;
        
        public double Length
        {
            get
            {
                EnsureLength();
                return _length.Value;
            }
        }

        public LanguageModel()
        {
            _trigramModel = new SortedList<char, SortedList<char, SortedList<char, long>>>();
            _length = null;
        }

        /// <summary>
        /// Convenience constructor that immediately loads and parses a model based on supplied text
        /// </summary>
        public LanguageModel(string text) : this()
        {
            ParseStream(new StreamReader(new MemoryStream(Encoding.Default.GetBytes(text))));
        }

        public void ParseStream(StreamReader reader)
        {
            _length = null; // basically an 'isDirty' flag

            //this is a 'trigram' model so just hard code first two characters
            if (reader.EndOfStream) return; // too short == no trigrams 
            char first = (char)reader.Read();
            
            if (reader.EndOfStream) return;
            char second = (char)reader.Read(); // too short == no trigrams 

            while (!reader.EndOfStream)
            {
                char third = (char)reader.Read();

                // wishin' this were python about now ;-)
                // ..initialize data structures if needed
                if (!_trigramModel.ContainsKey(first))
                    _trigramModel.Add(first, new SortedList<char, SortedList<char, long>>());
                if (!_trigramModel[first].ContainsKey(second))
                    _trigramModel[first].Add(second, new SortedList<char, long>());

                // ...increment, or initialize count for this trigram
                if (!_trigramModel[first][second].ContainsKey(third))
                    _trigramModel[first][second].Add(third, 1);
                else
                    _trigramModel[first][second][third]++;

                // .. cycle through characters
                first = second;
                second = third;
            }
        }

        /// <summary>
        /// Compute similarity between two trigram models. 
        /// For speed, you should call this on the smallest model
        /// and pass the largest model.. eg twitterStatusModel.Similarity(englishModel) 
        /// </summary>
        public double Similarity(LanguageModel other)
        {
            double tmp = 0;
            foreach (var first in _trigramModel)
            {
                if (!other._trigramModel.ContainsKey(first.Key)) continue;
                var otherFirst = other._trigramModel[first.Key];
                foreach (var second in first.Value)
                {
                    if (!otherFirst.ContainsKey(second.Key)) continue;
                    var otherSecond = otherFirst[second.Key];
                    foreach (var third in second.Value)
                    {
                        if (!otherSecond.ContainsKey(third.Key)) continue;
                        tmp += third.Value * otherSecond[third.Key];
                    }
                }
            }
            return tmp / (this.Length * other.Length);
        }

        private void EnsureLength()
        {
            if (_length.HasValue) return;

            double tmp = 0;
            foreach (var first in _trigramModel)
            {
                foreach (var second in first.Value)
                {
                    foreach (var third in second.Value)
                    {
                        tmp += third.Value * third.Value; 
                    }
                }
            }
            _length = Math.Sqrt(tmp);
        }
    }
}
