using System;
using System.IO;
using System.Reflection;
using OpenTextSummarizerCore.Interfaces;

namespace OpenTextSummarizerCore
{
    public class SummarizerArguments : ISummarizerArguments
    {
        public int FilteringConceptsCap { get; set; }

        public int MaxSummarySentences { get; set; }

        public int MaxSummarySizeInPercent { get; set; }

        public string Language { get; set; }

        public string DictionaryDirectory { get; set; }

        public SummarizerArguments()
        {
            Language = "en";
            DictionaryDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "","dics");
            FilteringConceptsCap = 5;
            MaxSummarySentences = 10;
            MaxSummarySizeInPercent = 10;

            ContentParser = () => new ClassicContentParser(Rules, new TextUnitBuilder(Rules));
            ContentAnalyzer = () => new ClassicContentAnalyzer(Rules);
            ContentSummarizer = () => new ClassicContentSummarizer();
        }

        private Dictionary rules;
        private readonly object rulesLock = new object();

        internal Dictionary Rules
        {
            get
            {
                // ReSharper disable once InvertIf
                if (rules == null)
                {
                    lock (rulesLock)
                    {
                        if (rules == null)
                        {
                            rules = Dictionary.LoadFromFile(Language, DictionaryDirectory);
                        }
                    }
                }
                return rules;
            }
        }

        public Func<IContentParser> ContentParser { get; set; }

        public Func<IContentAnalyzer> ContentAnalyzer { get; set; }

        public Func<IContentSummarizer> ContentSummarizer { get; set; }
    }
}