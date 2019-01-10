using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace OpenTextSummarizerCore
{
    internal class Dictionary
    {
        public List<string> UnimportantWords { get; set; }

        public List<string> LineBreakRules { get; set; }

        public List<string> NotALineBreakRules { get; set; }

        public List<string> DepreciateValueRule { get; set; }

        public List<string> TermFreqMultiplierRule { get; set; }

        //the replacement rules are stored as KeyValuePair<string,string>s
        //the Key is the search term. the Value is the replacement term
        public Dictionary<string, string> Step1PrefixRules { get; set; }

        public Dictionary<string, string> Step1SuffixRules { get; set; }

        public Dictionary<string, string> ManualReplacementRules { get; set; }

        public Dictionary<string, string> PrefixRules { get; set; }

        public Dictionary<string, string> SuffixRules { get; set; }

        public Dictionary<string, string> SynonymRules { get; set; }

        public static Dictionary LoadFromFile(string dictionaryLanguage, string dictionaryDirectory)
        {
            string dictionaryFile = Path.Combine(dictionaryDirectory, $"{dictionaryLanguage}.xml");
            if (!File.Exists(dictionaryFile))
            {
                throw new FileNotFoundException("Could Not Load Dictionary: " + dictionaryFile);
            }
            Dictionary dict = new Dictionary();
            XElement doc = XElement.Load(dictionaryFile);
            dict.Step1PrefixRules = LoadKeyValueRule(doc, "stemmer", "step1_pre");
            dict.Step1SuffixRules = LoadKeyValueRule(doc, "stemmer", "step1_post");
            dict.ManualReplacementRules = LoadKeyValueRule(doc, "stemmer", "manual");
            dict.PrefixRules = LoadKeyValueRule(doc, "stemmer", "pre");
            dict.SuffixRules = LoadKeyValueRule(doc, "stemmer", "post");
            dict.SynonymRules = LoadKeyValueRule(doc, "stemmer", "synonyms");
            dict.LineBreakRules = LoadValueOnlyRule(doc, "parser", "linebreak");
            dict.NotALineBreakRules = LoadValueOnlyRule(doc, "parser", "linedontbreak");
            dict.DepreciateValueRule = LoadValueOnlyRule(doc, "grader-syn", "depreciate");
            dict.TermFreqMultiplierRule = LoadValueOnlySection(doc, "grader-tf");

            dict.UnimportantWords = new List<string>();
            List<string> unimplementedWords = LoadValueOnlySection(doc, "grader-tc");
            foreach (string unimplementedWord in unimplementedWords)
            {
                dict.UnimportantWords.Add(unimplementedWord);
            }
            return dict;
        }

        private static List<string> LoadValueOnlySection(XContainer doc, string section)
        {
            IEnumerable<XElement> step1Pre = doc.Elements(section);
            return step1Pre.Elements().Select(x => x.Value).ToList();
        }

        private static List<string> LoadValueOnlyRule(XContainer doc, string section, string container)
        {
            IEnumerable<XElement> step1Pre = doc.Elements(section).Elements(container);
            return step1Pre.Elements().Select(x => x.Value).ToList();
        }

        private static Dictionary<string, string> LoadKeyValueRule(XContainer doc, string section, string container)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            IEnumerable<XElement> step1Pre = doc.Elements(section).Elements(container);
            foreach (XElement x in step1Pre.Elements())
            {
                string rule = x.Value;
                string[] keyValue = rule.Split('|');
                if (!dictionary.ContainsKey(keyValue[0]))
                    dictionary.Add(keyValue[0], keyValue[1]);
            }
            return dictionary;
        }
    }
}