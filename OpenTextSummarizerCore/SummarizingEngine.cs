using System;
using System.Collections.Generic;
using System.Linq;
using OpenTextSummarizerCore.Interfaces;

namespace OpenTextSummarizerCore
{
    /// <summary>
    /// Orchestrates the different parts of the summarizing algorithm
    /// </summary>
    internal class SummarizingEngine
    {
        /// <summary>
        /// Runs the content parsing part of the summarizing algorithm
        /// </summary>
        /// <param name="contentProvider"></param>
        /// <param name="contentParser"></param>
        /// <returns></returns>
        public ParsedDocument ParseContent(IContentProvider contentProvider, IContentParser contentParser)
        {
            if (contentProvider == null)
            {
                throw new ArgumentNullException(nameof(contentProvider));
            }
            if (contentParser == null)
            {
                throw new ArgumentNullException(nameof(contentParser));
            }

            ParsedDocument resultingParsedDocument = new ParsedDocument
            {
                Sentences = contentParser.SplitContentIntoSentences(contentProvider.Content)
            };
            if (resultingParsedDocument.Sentences == null)
            {
                throw new InvalidOperationException($"{contentProvider.GetType().FullName}.SplitContentIntoSentences must not return null");
            }
            foreach (Sentence workingSentence in resultingParsedDocument.Sentences)
            {
                workingSentence.TextUnits = contentParser.SplitSentenceIntoTextUnits(workingSentence.OriginalSentence);
                if (workingSentence.TextUnits == null)
                {
                    throw new InvalidOperationException($"{contentProvider.GetType().FullName}.SplitSentenceIntoTextUnits must not return null");
                }
            }
            return resultingParsedDocument;
        }

        /// <summary>
        /// Runs the content analysis part of the summarizing algorithm
        /// </summary>
        /// <param name="parsedDocument"></param>
        /// <param name="contentAnalyzer"></param>
        /// <returns></returns>
        public AnalyzedDocument AnalyzeParsedContent(ParsedDocument parsedDocument, IContentAnalyzer contentAnalyzer)
        {
            if (parsedDocument == null)
            {
                throw new ArgumentNullException(nameof(parsedDocument));
            }
            if (contentAnalyzer == null)
            {
                throw new ArgumentNullException(nameof(contentAnalyzer));
            }

            List<TextUnitScore> importantTextUnits = contentAnalyzer.GetImportantTextUnits(parsedDocument.Sentences);
            if (importantTextUnits == null)
            {
                throw new InvalidOperationException($"{contentAnalyzer.GetType().FullName}.GetImportantTextUnits must not return null");
            }
            List<SentenceScore> scoredSentences = contentAnalyzer.ScoreSentences(parsedDocument.Sentences, importantTextUnits);
            if (scoredSentences == null)
            {
                throw new InvalidOperationException($"{contentAnalyzer.GetType().FullName}.ScoreSentences must not return null");
            }

            return new AnalyzedDocument { ScoredTextUnits = importantTextUnits.OrderByDescending(tus => tus.Score).ToList(), ScoredSentences = scoredSentences.OrderByDescending(ss => ss.Score).ToList() };
        }

        /// <summary>
        /// Runs the content summarizing part of the summarizing algorithm
        /// </summary>
        /// <param name="analyzedDocument"></param>
        /// <param name="contentSummarizer"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public SummarizedDocument SummarizeAnalyzedContent(AnalyzedDocument analyzedDocument, IContentSummarizer contentSummarizer, ISummarizerArguments arguments)
        {
            if (analyzedDocument == null)
            {
                throw new ArgumentNullException(nameof(analyzedDocument));
            }

            if (contentSummarizer == null)
            {
                throw new ArgumentNullException(nameof(contentSummarizer));
            }

            if (arguments == null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            // Range adjustment
            if (arguments.FilteringConceptsCap < 0)
            {
                arguments.FilteringConceptsCap = 0;
            }

            if (arguments.MaxSummarySentences < 0)
            {
                arguments.MaxSummarySentences = 0;
            }

            if (arguments.MaxSummarySizeInPercent < 0)
            {
                arguments.MaxSummarySizeInPercent = 0;
            }

            if (arguments.MaxSummarySizeInPercent > 100)
            {
                arguments.MaxSummarySizeInPercent = 100;
            }

            List<string> summarizedConcepts = contentSummarizer.GetConcepts(analyzedDocument, arguments);
            if (summarizedConcepts == null)
            {
                throw new InvalidOperationException($"{contentSummarizer.GetType().FullName}.GetConcepts must not return null");
            }

            List<string> summarizedSentences = contentSummarizer.GetSentences(analyzedDocument, arguments);
            if (summarizedSentences == null)
            {
                throw new InvalidOperationException($"{contentSummarizer.GetType().FullName}.GetSentences must not return null");
            }

            return new SummarizedDocument { Concepts = summarizedConcepts, Sentences = summarizedSentences };
        }
    }
}