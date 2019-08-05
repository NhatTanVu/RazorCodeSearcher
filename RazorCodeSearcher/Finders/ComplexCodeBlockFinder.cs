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
            outputBlock = new ComplexCodeBlock(keywords, filePath);
            bool isFound = true;
            int fileLength = fileContent.Count;
            for (int i = 0; i < keywords.Length; i++)
            {
                string keyword = keywords[i];
                bool isExactlyFrom = !keyword.StartsWith(CodeBlock.ELLIPSIS);
                bool isOptional = keyword.EndsWith(CodeBlock.ASTERISK.ToString());
                CodeBlock block;
                if (fromLine < fileLength && toLine < fileLength)
                {
                    string realKeyword = keyword.Replace(CodeBlock.ELLIPSIS, string.Empty).TrimEnd(CodeBlock.ASTERISK);
                    if (innerFinder.FindFirst(realKeyword, filePath, fileContent, isExactlyFrom, out block, fromLine, toLine))
                    {
                        fromLine = block.EndLine + 1;
                        outputBlock.AddBlock(block);
                    }
                    else if(!isOptional)
                    {
                        isFound = false;
                        break;
                    }
                }
                else if(!isOptional)
                {
                    isFound = false;
                    break;
                }
            }
            if (!isFound)
            {
                outputBlock = new ComplexCodeBlock(keywords, filePath);
            }
            else if (!outputBlock.HasContent)
                isFound = false;
            return isFound;
        }

        public bool FindAll(string[] keywords, string filePath, List<string> fileContent, out ComplexCodeBlock outputBlock, int fromLine = 0, int toLine = -1)
        {
            if (toLine == -1)
                toLine = fileContent.Count - 1;
            outputBlock = new ComplexCodeBlock(keywords, filePath);
            ComplexCodeBlock firstBlock;
            bool isFound = FindFirst(keywords, filePath, fileContent, out firstBlock, fromLine, toLine);
            if (isFound)
            {
                outputBlock.AddBlock(firstBlock);
                if (firstBlock.EndLine < toLine)
                {
                    ComplexCodeBlock nextBlock;
                    int fromNextLine = firstBlock.EndLine + 1;
                    while (fromNextLine < toLine)
                    {
                        if (FindFirst(keywords, filePath, fileContent, out nextBlock, fromNextLine, toLine))
                        {
                            outputBlock.AddBlock(nextBlock);
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
