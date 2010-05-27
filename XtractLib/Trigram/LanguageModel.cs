using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace XtractLib.Trigram
{
    public class LanguageModel 
    {
        private double? _length;
        private readonly SortedList<char, SortedList<char, SortedList<char, int>>> _trigramModel;
        
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
            _trigramModel = new SortedList<char, SortedList<char, SortedList<char, int>>>();
            _length = null;
        }

        public void ParseStream(StreamReader reader)
        {
            _length = null; // basically an 'isDirty' flag

            //this is a 'trigram' model so just hard code three levels
            if (reader.EndOfStream) return;
            char first = (char)reader.Read();
            
            if (reader.EndOfStream) return;
            char second = (char)reader.Read();

            while (!reader.EndOfStream)
            {
                char third = (char)reader.Read();

                // wishin' this were python about now ;-)
                if (_trigramModel[first] == null)
                    _trigramModel[first] = new SortedList<char, SortedList<char, int>>();
                if (_trigramModel[first][second] == null)
                    _trigramModel[first][second] = new SortedList<char, int>();

                _trigramModel[first][second][third]++;

                first = second;
                second = third;
            }
        }

        public double Similarity(LanguageModel other)
        {
            double tmp = 0;
            foreach (var first in _trigramModel)
            {
                if (!other._trigramModel.ContainsKey(first.Key)) break;
                var otherFirst = other._trigramModel[first.Key];
                foreach (var second in first.Value)
                {
                    if (!otherFirst.ContainsKey(second.Key)) break;
                    var otherSecond = otherFirst[second.Key];
                    foreach (var third in second.Value)
                    {
                        if (!otherSecond.ContainsKey(third.Key)) break;
                        tmp += third.Value * otherSecond[third.Key];
                    }
                }
            }
            return tmp;
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
