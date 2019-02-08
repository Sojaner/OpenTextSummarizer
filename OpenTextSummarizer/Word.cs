using System;

namespace OpenTextSummarizer
{
    internal class Word
    {
        public string Value { get; }

        public string Stem { get; set; }

        public double TermFrequency { get; set; }

        public Word(string word) { Value = word; }

        public override bool Equals(object obj)
        {
            if (obj != null && GetType() != obj.GetType()) return false;

            Word arg = (Word)obj;

            return arg != null && Value.Equals(arg.Value, StringComparison.CurrentCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
