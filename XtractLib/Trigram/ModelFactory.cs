using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace XtractLib.Trigram
{
    public static class ModelFactory
    {
        public static LanguageModel LoadModelFromFolder(string folderPath)
        {
            LanguageModel model = new LanguageModel();
            string[] filenames = Directory.GetFiles(folderPath, "*.txt");
            foreach (string filename in filenames)
            {
               
                using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF32))
                    {
                        model.ParseStream(reader);
                    }
                }
                Console.Out.Write("Done parsing file: " + filename);
                Console.Out.WriteLine(" Length: "+ model.Length);
            }
            return model;
        }
    }
}
