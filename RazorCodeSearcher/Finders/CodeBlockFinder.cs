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
            outputBlock = new CodeBlock(new string[] { keyword }, filePath);
            bool isFound = false;
            if (isExactlyFrom)
            {
                string line = fileContent[fromLine];
                string trimLine = line.Replace(" ", string.Empty).Replace("\t", string.Empty);
                if (trimLine.StartsWith(keyword))
                {
                    isFound = true;
                    outputBlock.StartLine = outputBlock.EndLine = fromLine;
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
                        outputBlock.StartLine = outputBlock.EndLine = i;
                        isFound = true;
                        break;
                    }
                }
            }
            if (isFound)
            {
                string trimLine;
                for (int i = outputBlock.StartLine + 1; i <= toLine; i++)
                {
                    string line = fileContent[i];
                    trimLine = line.Replace(" ", string.Empty).Replace("\t", string.Empty);
                    if (trimLine.StartsWith(keyword) || string.IsNullOrEmpty(trimLine))
                    {
                        outputBlock.EndLine = i;
                    }
                    else
                        break;
                }

                if (outputBlock.EndLine < toLine)
                {
                    trimLine = fileContent[outputBlock.EndLine].Replace(" ", string.Empty).Replace("\t", string.Empty);
                    string nextTrimLine = fileContent[outputBlock.EndLine + 1].Replace(" ", string.Empty).Replace("\t", string.Empty);
                    if (trimLine.StartsWith(CodeBlock.BEGIN_COMMENT_BLOCK))
                    {
                        for (int i = outputBlock.EndLine; i <= toLine; i++)
                        {
                            string trimLine2 = fileContent[i].Replace(" ", string.Empty).Replace("\t", string.Empty);
                            if (trimLine2.EndsWith(CodeBlock.END_COMMENT_BLOCK))
                            {
                                outputBlock.EndLine = i;
                                break;
                            }
                        }
                    }
                    else if (trimLine.EndsWith(CodeBlock.BEGIN_BLOCK) || nextTrimLine.EndsWith(CodeBlock.BEGIN_BLOCK))
                    {
                        int numOfBeginBlock = 1;
                        int i = trimLine.EndsWith(CodeBlock.BEGIN_BLOCK) ? outputBlock.EndLine + 1 : outputBlock.EndLine + 2;
                        for (; i <= toLine && numOfBeginBlock > 0; i++)
                        {
                            string trimLine2 = fileContent[i].Replace(" ", string.Empty).Replace("\t", string.Empty);
                            if (trimLine2.Contains(CodeBlock.BEGIN_BLOCK))
                                numOfBeginBlock++;
                            if (trimLine2.Contains(CodeBlock.END_BLOCK))
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
                }
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
