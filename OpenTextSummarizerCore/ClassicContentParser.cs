using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTextSummarizerCore.Interfaces;

namespace OpenTextSummarizerCore
{
    internal class ClassicContentParser : IContentParser
    {
        internal Dictionary Rules { get; set; }

        public ITextUnitBuilder TextUnitBuilder { get; set; }

        public ClassicContentParser(Dictionary rules, ITextUnitBuilder textUnitBuilder)
        {
            Rules = rules;
            TextUnitBuilder = textUnitBuilder;
        }

        public List<Sentence> SplitContentIntoSentences(string content)
        {
            List<Sentence> listSentences = new List<Sentence>();
            if (string.IsNullOrEmpty(content))
            {
                return listSentences;
            }

            string[] words = content.Split(' ', '\r'); //space and line feed characters are the ends of words.
            Sentence sentence = new Sentence { OriginalSentenceIndex = listSentences.Count };
            listSentences.Add(sentence);
            StringBuilder originalSentence = new StringBuilder();
            foreach (string word in words)
            {
                string locWord = word;
                if (locWord.StartsWith("\n") && word.Length > 2) locWord = locWord.Replace("\n", "");

                if (IsSentenceBreak(locWord))
                {
                    originalSentence.AppendFormat("{0}", locWord);
                    sentence.OriginalSentence = originalSentence.ToString();
                    sentence = new Sentence { OriginalSentenceIndex = listSentences.Count };
                    originalSentence = new StringBuilder();
                    listSentences.Add(sentence);
                }
                else
                {
                    originalSentence.AppendFormat("{0} ", locWord);
                }
            }
            sentence.OriginalSentence = originalSentence.ToString();
            return listSentences;
        }

        private bool IsSentenceBreak(string word)
        {
            if (word.Contains("\r") || word.Contains("\n")) return true;
            bool shouldBreak = Rules.LineBreakRules
                                   .Where(p => word.EndsWith(p, StringComparison.CurrentCultureIgnoreCase))
                                   .Count() > 0;

            if (shouldBreak == false) return shouldBreak;

            shouldBreak = (Rules.NotALineBreakRules
                .Where(p => word.StartsWith(p, StringComparison.CurrentCultureIgnoreCase))
                .Count() == 0);

            return shouldBreak;
        }

        public List<TextUnit> SplitSentenceIntoTextUnits(string sentence)
        {
            List<TextUnit> listUnits = new List<TextUnit>();
            if (string.IsNullOrEmpty(sentence))
            {
                return listUnits;
            }

            foreach (string word in sentence.Split(' ', '\r'))
            {
                listUnits.Add(TextUnitBuilder.Build(word));
            }

            return listUnits;
        }
    }
}