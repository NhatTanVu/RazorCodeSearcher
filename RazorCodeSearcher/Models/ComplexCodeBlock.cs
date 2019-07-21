using System.Collections.Generic;
using System.Linq;

namespace RazorCodeSearcher.Models
{
    public class ComplexCodeBlock : CodeBlock
    {
        public const string BETWEEN_BLOCKS_SEPARATOR = ".........";
        private List<CodeBlock> codeBlocks = new List<CodeBlock>();

        public override int StartLine
        {
            get
            {
                return codeBlocks.First().StartLine;
            }
        }

        public override int EndLine
        {
            get
            {
                return codeBlocks.Last().EndLine;
            }
        }

        public void AddBlock(CodeBlock block)
        {
            if (HasContent)
                Content.Add(new KeyValuePair<string, string>("xxx", BETWEEN_BLOCKS_SEPARATOR));
            codeBlocks.Add(block);
            Content.AddRange(block.Content);
        }
    }
}
