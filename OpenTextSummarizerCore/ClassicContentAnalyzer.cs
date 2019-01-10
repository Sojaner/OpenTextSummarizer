using System.Collections.Generic;
using System.Linq;
using OpenTextSummarizerCore.Interfaces;

namespace OpenTextSummarizerCore
{
    /// <summary>
    /// Analyzer class to determine important text units and score sentences
    /// </summary>
    internal class ClassicContentAnalyzer : IContentAnalyzer
    {
        public Dictionary MRules { get; set; }

        public ClassicContentAnalyzer(Dictionary rules)
        {
            MRules = rules;
        }

        public List<TextUnitScore> GetImportantTextUnits(List<Sentence> sentences)
        {
            Dictionary<TextUnit, long> textUnitFrequencyGrader = new Dictionary<TextUnit, long>();
            foreach (TextUnit tu in sentences.SelectMany(s => s.TextUnits))
            {
                if (MRules.UnimportantWords.Contains(tu.FormattedValue))
                {
                    continue;
                }

                if (textUnitFrequencyGrader.ContainsKey(tu))
                {
                    textUnitFrequencyGrader[tu]++;
                }
                else
                {
                    textUnitFrequencyGrader.Add(tu, 1);
                }
            }

            return textUnitFrequencyGrader.OrderByDescending(kvp => kvp.Value).Select(kvp => new TextUnitScore { ScoredTextUnit = kvp.Key, Score = kvp.Value }).ToList();
        }

        public List<SentenceScore> ScoreSentences(List<Sentence> sentences, List<TextUnitScore> importantTextUnits)
        {
            List<string> stemList = importantTextUnits.Select(tus => tus.ScoredTextUnit.Stem).Distinct().ToList();
            List<SentenceScore> listSentenceScorer = new List<SentenceScore>();
            foreach (Sentence s in sentences.Where(s => s.TextUnits.Count > 2))
            {
                SentenceScore newSentenceScorer = new SentenceScore();
                newSentenceScorer.ScoredSentence = s;
                newSentenceScorer.Score = newSentenceScorer.ScoredSentence.TextUnits.Count(tu => stemList.Contains(tu.Stem));

                if (s.TextUnits[0].RawValue.Contains("\n") && s.TextUnits[1].RawValue.Contains("\n"))
                {
                    newSentenceScorer.Score *= 1.6;
                }

                listSentenceScorer.Add(newSentenceScorer);
            }

            // additional scoring
            if (listSentenceScorer.Any())
            {
                listSentenceScorer.First().Score *= 2;
            }

            return listSentenceScorer.OrderByDescending(ss => ss.Score).ToList();
        }
    }
}