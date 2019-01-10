namespace OpenTextSummarizerCore
{
    public class TextUnit
    {
        public string RawValue { get; set; }

        public string FormattedValue { get; set; }

        public string Stem { get; set; }

        public override int GetHashCode()
        {
            return Stem == null ? 0 : Stem.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is TextUnit other && other.Stem == Stem;
        }
    }
}