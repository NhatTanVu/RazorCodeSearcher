using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RazorCodeSearcher.Models
{
    public class SearchResult
    {
        public const string SEARCH_RESULT_SEPARATOR = "------------------------------------";
        public const string BLOCK_SEPARATOR = "------------------------------------";
        public const string KEYWORDS_SEPARATOR = ", ";
        protected List<ComplexCodeBlock> blocks = new List<ComplexCodeBlock>();

        public SearchResult() { }
        public SearchResult(ComplexCodeBlock[] blocks)
        {
            this.blocks = new List<ComplexCodeBlock>(blocks);
        }
        public virtual bool HasContent()
        {
            return this.blocks.Count > 0 && this.blocks.All(b => b.HasContent);
        }
    }
}