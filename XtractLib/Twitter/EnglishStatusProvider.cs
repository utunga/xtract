

using System.Collections.Generic;
using XtractLib.Trigram;

namespace XtractLib.Twitter
{
    public class EnglishStatusProvider : IMessageProvider<TwitterStatus>
    {
        readonly IMessageProvider<TwitterStatus> _statusProvider;
        readonly LanguageModel _english;

        public double Threshold { get; set; }

        public EnglishStatusProvider(IMessageProvider<TwitterStatus> statusProvider, string directoryWithEnglishLanguageTextFiles)
        {
            _statusProvider = statusProvider;
            Threshold = 1.5d; 
            _english = ModelFactory.LoadModelFromFolder(directoryWithEnglishLanguageTextFiles);
        }

        public IEnumerable<TwitterStatus> GetMessages()
        {
            foreach (TwitterStatus status in _statusProvider.GetMessages())
            {
                if (status.user != null && status.user.lang == "en")
                {
                    LanguageModel smallModel = new LanguageModel(status.text);
                    double similarity  = smallModel.Similarity(_english);
                    if (similarity > Threshold)
                    {
                        status.english_similarity = similarity;
                        yield return status;
                    }
                }
            }
        }

    }
}
