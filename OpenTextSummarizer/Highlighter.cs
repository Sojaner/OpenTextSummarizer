using System.Collections.Generic;
using System.Linq;

namespace OpenTextSummarizer
{
    internal class Highlighter
    {
        internal static void Highlight(Article article, SummarizerArguments args)
        {
            switch (args.DisplayPercent)
            {
                case 0 when args.DisplayLines == 0:

                    return;

                case 0:

                    //get the highest scored n lines, without reordering the list.
                    SelectNumberOfSentences(article, args.DisplayLines);

                    break;

                default:

                    SelectSentencesByPercent(article, args.DisplayPercent);

                    break;
            }
        }

        private static void SelectSentencesByPercent(Article article, int percent)
        {
            if(percent > 100) percent = 100;

            if(percent < 1) percent = 1;

            IEnumerable<Sentence> sentencesByScore = article.Sentences.OrderByDescending(p => p.Score).Select(p => p);

            int totalWords = article.Sentences.Sum(p => p.Words.Count);

            int maxWords = (int) (totalWords*(percent/100f));

            int wordsCount = 0;

            foreach (Sentence sentence in sentencesByScore)
            {
                if (sentence.OriginalSentence == null) continue;

                sentence.Selected = true;

                wordsCount += sentence.Words.Count;

                if (wordsCount >= maxWords) break;
            }

        }

        private static void SelectNumberOfSentences(Article article, int lineCount)
        {
            IEnumerable<Sentence> sentencesByScore = article.Sentences.OrderByDescending(p => p.Score).Select(p => p);

            int loopCounter = 0;

            foreach (Sentence sentence in sentencesByScore)
            {
                if (sentence.OriginalSentence == null) continue;

                sentence.Selected = true;

                loopCounter++;

                if (loopCounter >= lineCount) break;
            }
        }
    }
}
