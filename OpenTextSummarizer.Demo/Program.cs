using System;
using System.IO;
using System.Reflection;

namespace OpenTextSummarizer.Demo
{
    internal class Program
    {
        private static void Main()
        {
            SummarizerArguments summarizerArguments = new SummarizerArguments
            {
                InputFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TextualData", "AutomaticSummarization.txt")
            };

            SummarizedDocument summarizedDocument = Summarizer.Summarize(summarizerArguments);

            string summery = string.Join(Environment.NewLine, summarizedDocument.Sentences);

            Console.Write(summery);

            Console.ReadLine();
        }
    }
}
