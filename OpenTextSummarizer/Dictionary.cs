using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace OpenTextSummarizer
{
    internal class Dictionary
    {
        public List<Word> UnimportantWords { get; set; }

        public List<string> LinebreakRules { get; set; }

        public List<string> NotALinebreakRules { get; set; }

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

        public string Language { get; set; }

        private Dictionary(){}

        public static Dictionary LoadFromFile(string dictionaryLanguage)
        {
            string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? Environment.CurrentDirectory;

            directoryName = Path.Combine(directoryName, "dics");

            return LoadFromFile(new DirectoryInfo(directoryName), dictionaryLanguage);
        }

        public static Dictionary LoadFromFile(DirectoryInfo dictionariesDirectoryInfo, string dictionaryLanguage)
        {
            string dictionaryFile = Path.Combine(dictionariesDirectoryInfo.FullName, $"{dictionaryLanguage}.xml");

            if(!File.Exists(dictionaryFile))
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

            dict.LinebreakRules = LoadValueOnlyRule(doc, "parser", "linebreak");

            dict.NotALinebreakRules = LoadValueOnlyRule(doc, "parser", "linedontbreak");

            dict.DepreciateValueRule = LoadValueOnlyRule(doc, "grader-syn", "depreciate");

            dict.TermFreqMultiplierRule = LoadValueOnlySection(doc, "grader-tf");

            dict.UnimportantWords = new List<Word>();

            List<string> unimpwords = LoadValueOnlySection(doc, "grader-tc");

            foreach (string unimpword in unimpwords)
            {
                dict.UnimportantWords.Add(new Word(unimpword));
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
            IEnumerable<XElement> step1Pre = doc.Elements(section).Elements(container);

            return step1Pre.Select(element => new {Key = element.Value.Split('|')[0], Value = element.Value.Split('|')[1]})
                
                .GroupBy(arg => arg.Key, arg => arg.Value, (key, values) => new {Key = key, Value = values.First()})
                
                .ToDictionary(arg => arg.Key, arg => arg.Value);
        }

    }
}
