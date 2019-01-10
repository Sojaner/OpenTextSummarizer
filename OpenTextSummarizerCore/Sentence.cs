using System.Collections.Generic;

namespace OpenTextSummarizerCore
{
    public class Sentence
    {
        public string OriginalSentence { get; set; }

        public long OriginalSentenceIndex { get; set; }

        public List<TextUnit> TextUnits { get; set; }
    }
}