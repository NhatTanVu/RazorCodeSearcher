using RazorCodeSearcher.Models;
using System.Collections.Generic;
using System.Linq;

namespace RazorCodeSearcher.Finders
{
    public class CodeBlockFinder
    {
        public bool FindFirst(string keyword, string filePath, List<string> fileContent, bool isExactlyFrom, out CodeBlock outputBlock, int fromLine = 0, int toLine = -1)
        {
            if (toLine == -1)
                toLine = fileContent.Count - 1;
            outputBlock = new CodeBlock();
            outputBlock.Keywords = new string[] { keyword };
            outputBlock.FilePath = filePath;
            bool isFound = false;
            if (isExactlyFrom)
            {
                string line = fileContent[fromLine];
                string trimLine = line.Replace(" ", string.Empty).Replace("\t", string.Empty);
                if (trimLine.StartsWith(keyword))
                {
                    isFound = true;
                    outputBlock.StartLine = fromLine;
                }
            }
            else
            {
                for (int i = fromLine; i <= toLine; i++)
                {
                    string line = fileContent[i];
                    string trimLine = line.Replace(" ", string.Empty).Replace("\t", string.Empty);
                    if (trimLine.StartsWith(keyword))
                    {
                        outputBlock.StartLine = i;
                        isFound = true;
                        break;
                    }
                }
            }
            if (isFound)
            {
                string trimLine = fileContent[outputBlock.StartLine].Replace(" ", string.Empty).Replace("\t", string.Empty);
                if (trimLine.StartsWith(keyword + CodeBlock.BEGIN_BLOCK) ||
                    ((keyword == CodeBlock.BEGIN_BLOCK || keyword == CodeBlock.BEGIN_BLOCK_2_0) && !trimLine.EndsWith(CodeBlock.END_BLOCK)))
                {
                    int numOfBeginBlock = 1;
                    for (int i = outputBlock.StartLine + 1; i <= toLine && numOfBeginBlock > 0; i++)
                    {
                        string trimLine2 = fileContent[i].Replace(" ", string.Empty).Replace("\t", string.Empty);
                        if (trimLine2.Contains(CodeBlock.BEGIN_BLOCK))
                            numOfBeginBlock++;
                        else if (trimLine2.Contains(CodeBlock.END_BLOCK))
                            numOfBeginBlock--;

                        if (numOfBeginBlock == 0)
                        {
                            outputBlock.EndLine = i;
                            break;
                        }
                    }
                    if (numOfBeginBlock > 0)
                    {
                        outputBlock.EndLine = -1;
                        isFound = false;
                    }
                }
                else
                    outputBlock.EndLine = outputBlock.StartLine;
            }
            if (isFound)
            {
                for (int i = outputBlock.StartLine; i <= outputBlock.EndLine; i++)
                    outputBlock.Content.Add(new KeyValuePair<string, string>((i + 1).ToString(), fileContent[i]));
            }
            return isFound;
        }
    }
}
