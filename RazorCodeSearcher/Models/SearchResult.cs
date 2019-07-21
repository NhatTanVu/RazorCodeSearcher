using System;
using System.Linq;

namespace RazorCodeSearcher.Models
{
    public class SearchResult
    {
        public const string SEARCH_RESULT_SEPARATOR = "------------------------------------";
        public const string BLOCK_SEPARATOR = "------------------------------------";
        public const string KEYWORDS_SEPARATOR = ", ";
        private string[] listOfKeywords;
        private ComplexCodeBlock[] blocks;
        public string FilePath { get; set; }
        public SearchResult(string[] listOfKeywords, ComplexCodeBlock[] blocks)
        {
            this.listOfKeywords = listOfKeywords;
            this.blocks = blocks;
        }
        public bool HasContent()
        {
            return this.blocks.Length > 0 && this.blocks.All(b => b.HasContent);
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
