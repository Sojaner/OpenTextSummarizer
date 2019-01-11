using System;
using System.IO;
using System.Reflection;

namespace OpenTextSummarizerCore.Demo
{
    internal class Program
    {
        private static void Main()
        {
            SummarizedDocument summarizedDocument = Summarizer.Summarize(
                new FileContentProvider(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),"TextualData\\AutomaticSummarization.txt")),
                new SummarizerArguments
                {
                    Language = "en",
                    MaxSummarySentences = 5
                });

            string summery = string.Join(Environment.NewLine, summarizedDocument.Sentences);
            Console.Write(summery);
            Console.ReadLine();
        }
    }
}
