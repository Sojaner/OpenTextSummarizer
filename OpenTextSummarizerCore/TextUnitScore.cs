using System.Diagnostics;

namespace OpenTextSummarizerCore
{

    [DebuggerDisplay("Value: {ScoredTextUnit.FormattedValue} - Stem: {ScoredTextUnit.Stem} - Score: {Score}")]
    public class TextUnitScore
    {
        public TextUnit ScoredTextUnit { get; set; }

        public double Score { get; set; }
    }
}