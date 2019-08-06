using System;
using System.Linq;

namespace RazorCodeSearcher.Models
{
    public class KeywordsSearchResult : SearchResult
    {
        private string[] listOfKeywords;

        public string[] ListOfKeywords { get { return listOfKeywords; } }

        public KeywordsSearchResult(string[] listOfKeywords, ComplexCodeBlock[] blocks) : base(blocks)
        {
            this.listOfKeywords = listOfKeywords;
        }
        public override string ToString()
        {
            return string.Join(Environment.NewLine, new string[] {
                SEARCH_RESULT_SEPARATOR,
                "KEYWORDS: " + string.Join(SearchResult.KEYWORDS_SEPARATOR, listOfKeywords),
                HasContent() ? string.Join(Environment.NewLine, blocks.Select(b => b.ToString())) : (BLOCK_SEPARATOR + Environment.NewLine + "N/A"),
                SEARCH_RESULT_SEPARATOR
            });
        }

        public string GetFileName()
        {
            string fileName = string.Join(SearchResult.FILENAME_SEPARATOR, listOfKeywords.Select(k => k.Replace(CodeBlock.BEGIN_COMMENT_BLOCK, "comment").Replace(CodeBlock.CODE_GENERAL, string.Empty)
            .Replace(CodeBlock.ELLIPSIS, string.Empty).Replace(CodeBlock.ASTERISK.ToString(), string.Empty).Replace(".", SearchResult.FILENAME_SEPARATOR)));
            if (string.IsNullOrEmpty(fileName))
                fileName = "general";
            return fileName;
        }
    }
}
