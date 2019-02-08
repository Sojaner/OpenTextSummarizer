using System.IO;

namespace OpenTextSummarizer
{
    public class Summarizer
    {
        private Summarizer()
        {
        }

        public static SummarizedDocument Summarize(SummarizerArguments args)
        {
            if (args == null) return null;

            Article article;

            if (args.InputString.Length > 0 && args.InputFile.Length == 0)
            {
                article = ParseDocument(args.InputString, args);
            }
            else
            {
                article = ParseFile(args.InputFile, args);
            }

            Grader.Grade(article);

            Highlighter.Highlight(article, args);

            SummarizedDocument summarizedDocument = CreateSummarizedDocument(article);

            return summarizedDocument;

        }

        private static SummarizedDocument CreateSummarizedDocument(Article article)
        {
            SummarizedDocument sumDoc = new SummarizedDocument { Concepts = article.Concepts };

            foreach (Sentence sentence in article.Sentences)
            {
                if (sentence.Selected)
                {
                    sumDoc.Sentences.Add(sentence.OriginalSentence);
                }
            }

            return sumDoc;
        }

        private static Article ParseFile(string fileName, SummarizerArguments args)
        {
            string text = LoadFile(fileName);

            return ParseDocument(text, args);
        }

        private static Article ParseDocument(string text, SummarizerArguments args)
        {
            Dictionary rules = args.DictionariesDirectoryInfo == null ? Dictionary.LoadFromFile(args.DictionaryLanguage) : Dictionary.LoadFromFile(args.DictionariesDirectoryInfo, args.DictionaryLanguage);

            Article article = new Article(rules);

            article.ParseText(text);

            return article;
        }

        private static string LoadFile(string fileName)
        {
            return fileName != string.Empty ? File.ReadAllText(fileName) : string.Empty;
        }
    }
}
