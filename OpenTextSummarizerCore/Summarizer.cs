using OpenTextSummarizerCore.Interfaces;

namespace OpenTextSummarizerCore
{
    public class Summarizer
    {
        public static SummarizedDocument Summarize(IContentProvider contentProvider, ISummarizerArguments args)
        {
            if (contentProvider == null || args == null)
            {
                return new SummarizedDocument();
            }

            SummarizingEngine engine = new SummarizingEngine();

            ParsedDocument parsedDocument = engine.ParseContent(contentProvider, args.ContentParser());
            AnalyzedDocument analyzedDocument = engine.AnalyzeParsedContent(parsedDocument, args.ContentAnalyzer());
            SummarizedDocument summaryAnalysisDocument = engine.SummarizeAnalyzedContent(analyzedDocument, args.ContentSummarizer(), args);

            return summaryAnalysisDocument;
        }
    }
}