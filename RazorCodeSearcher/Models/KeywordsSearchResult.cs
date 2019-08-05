using System;
using System.Linq;

namespace RazorCodeSearcher.Models
{
    public class KeywordsSearchResult : SearchResult
    {
        private string[] listOfKeywords;
        public KeywordsSearchResult(string[] listOfKeywords, ComplexCodeBlock[] blocks): base(blocks)
        {
            this.listOfKeywords = listOfKeywords;
        }
        public override string ToString()
        {
            return string.Join(Environment.NewLine, new string[] {
                SEARCH_RESULT_SEPARATOR,
                "KEYWORDS: " + string.Join(KEYWORDS_SEPARATOR, listOfKeywords),
                HasContent() ? string.Join(Environment.NewLine, blocks.Select(b => b.ToString())) : (BLOCK_SEPARATOR + Environment.NewLine + "N/A"),
                SEARCH_RESULT_SEPARATOR
            });
        }
    }
}
