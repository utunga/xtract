using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XtractLib.Words;
using XtractLinq.Tasks;

namespace XtractLinq
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                PrintUsageAndExit();
            }

            ITask task=null;
            switch (args[0])
            {
                case "SampleStream":
                    task = new SampleStream();
                    break;

                case "GenerateWords":
                    IUrlExpander expander = new UrlExpander();
                    Tokenizer tokenizer = new Tokenizer(expander);
                    task = new GenerateWords(tokenizer);
                    break;
               
                case "PushToCouch":
                    IUrlExpander expander1 = new UrlExpander();
                    Tokenizer tokenizer1 = new Tokenizer(expander1);
                    task = new PushAllDataToCouch(tokenizer1);
                    break;

                case "UpdateSimilarityScores":
                    task = new UpdateSimilarityScores();
                    break;

                case "DownloadCandidatesData":
                    task = new DownloadCandidatesData();
                    break;

                default:
                    PrintUsageAndExit();
                    break; 
            }

            task.Execute();
            Environment.Exit(0); ;
        }

        static void PrintUsageAndExit()
        {
            Console.Out.WriteLine("Usage: Xtract [SampleStream|GenerateWords|PushToCouch|UpdateSimilarityScores|DownloadCandidatesData]");
            Environment.Exit(1);
        }
    }
}
