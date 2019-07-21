using RazorCodeSearcher.Models;
using System.Collections.Generic;

namespace RazorCodeSearcher.Finders
{
    public class ComplexCodeBlockFinder
    {
        private CodeBlockFinder innerFinder = new CodeBlockFinder();

        public bool FindFirst(string[] keywords, string filePath, List<string> fileContent, out ComplexCodeBlock outputBlock, int fromLine = 0, int toLine = -1)
        {
            if (toLine == -1)
                toLine = fileContent.Count - 1;
            outputBlock = new ComplexCodeBlock();
            outputBlock.Keywords = keywords;
            outputBlock.FilePath = filePath;
            bool isFound = true;
            int fileLength = fileContent.Count;
            for (int i = 0; i < keywords.Length; i++)
            {
                string keyword = keywords[i];
                bool isExactlyFrom = (i > 0);
                CodeBlock block;
                if (fromLine < fileLength & toLine < fileLength)
                {
                    if (innerFinder.FindFirst(keyword, filePath, fileContent, isExactlyFrom, out block, fromLine, toLine))
                    {
                        fromLine = block.EndLine + 1;
                        outputBlock.AddBlock(block);
                    }
                    else
                    {
                        isFound = false;
                        break;
                    }
                }
                else
                {
                    isFound = false;
                    break;
                }
            }
            if (!isFound)
            {
                outputBlock = new ComplexCodeBlock();
            }
            else if (!outputBlock.HasContent)
                isFound = false;
            return isFound;
        }

        public bool FindAll(string[] keywords, string filePath, List<string> fileContent, out List<ComplexCodeBlock> outputBlocks, int fromLine = 0, int toLine = -1)
        {
            if (toLine == -1)
                toLine = fileContent.Count - 1;
            outputBlocks = new List<ComplexCodeBlock>();
            ComplexCodeBlock firstBlock;
            bool isFound = FindFirst(keywords, filePath, fileContent, out firstBlock, fromLine, toLine);
            if (isFound)
            {
                outputBlocks.Add(firstBlock);
                if (firstBlock.EndLine < toLine)
                {
                    ComplexCodeBlock nextBlock;
                    int fromNextLine = firstBlock.EndLine + 1;
                    while (fromNextLine < toLine)
                    {
                        if (FindFirst(keywords, filePath, fileContent, out nextBlock, fromNextLine, toLine))
                        {
                            outputBlocks.Add(nextBlock);
                            fromNextLine = nextBlock.EndLine + 1;
                        }
                        else
                            break;
                    }
                }
            }
            return isFound;
        }
    }
}
