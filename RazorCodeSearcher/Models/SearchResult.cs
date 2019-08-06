using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RazorCodeSearcher.Models
{
    public class SearchResult
    {
        private DistanceMatrix matrix;
        public const string SEARCH_RESULT_SEPARATOR = "------------------------------------";
        
        public const string BLOCK_SEPARATOR = "------------------------------------";
        public const string KEYWORDS_SEPARATOR = ", ";
        public const string FILENAME_SEPARATOR = "_";
        protected List<ComplexCodeBlock> blocks = new List<ComplexCodeBlock>();

        public DistanceMatrix DistanceMatrix
        {
            get { return matrix; }
        }

        public SearchResult(ComplexCodeBlock[] blocks)
        {
            this.blocks = new List<ComplexCodeBlock>(blocks);
            this.matrix = new DistanceMatrix(blocks);
            this.CalculateDistances();
        }
        public virtual bool HasContent()
        {
            return this.blocks.Count > 0 && this.blocks.All(b => b.HasContent);
        }

        public void CalculateDistances()
        {
            this.matrix.Calculate();
        }
    }
}