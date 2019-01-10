using System.Collections.Generic;

namespace OpenTextSummarizerCore
{
    public class AnalyzedDocument
    {
        public List<TextUnitScore> ScoredTextUnits { get; set; }

        public List<SentenceScore> ScoredSentences { get; set; }
    }
}