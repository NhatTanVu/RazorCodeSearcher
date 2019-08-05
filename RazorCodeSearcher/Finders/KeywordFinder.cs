using RazorCodeSearcher.Models;
using System.Collections.Generic;

namespace RazorCodeSearcher.Finders
{
    public class KeywordFinder
    {
        public const char KEYWORD_START_TOKEN = '@';
        public const char CLASS_START_TOKEN = '.';
        public const char METHOD_START_TOKEN = '(';
        public const char ASTERISK = '*';

        public List<string> FindAll(string filePath, List<string> fileContent)
        {
            List<string> keywords = new List<string>();
            foreach (string line in fileContent)
            {
                string trimLine = line.Trim();
                if (trimLine.Length > 0 && trimLine[0] == KEYWORD_START_TOKEN)
                {
                    string keyword = string.Empty;
                    if (trimLine[1] == CodeBlock.ASTERISK)
                        keyword = "@" + CodeBlock.ASTERISK;
                    else if (trimLine[1].ToString() == CodeBlock.BEGIN_BLOCK)
                        keyword = "@" + CodeBlock.BEGIN_BLOCK;
                    else if (trimLine[1] == KeywordFinder.METHOD_START_TOKEN)
                        keyword = "@" + KeywordFinder.METHOD_START_TOKEN;
                    else if (trimLine.IndexOf("@if") == 0)
                        keyword = "@if";
                    else
                    {
                        bool isMethodCall = (trimLine.IndexOf(KeywordFinder.CLASS_START_TOKEN) > 0) && (trimLine.IndexOf(KeywordFinder.METHOD_START_TOKEN, trimLine.IndexOf(KeywordFinder.CLASS_START_TOKEN) + 1) > 0);
                        if (isMethodCall)
                        {
                            keyword = trimLine.Substring(0, trimLine.IndexOf(KeywordFinder.METHOD_START_TOKEN));
                        }
                        else
                        {
                            if (trimLine[1] == CodeBlock.ASTERISK)
                                keyword = "@*";
                            else if (trimLine.IndexOf(" ") > 0)
                                keyword = trimLine.Substring(0, trimLine.IndexOf(" "));
                            else if (trimLine.IndexOf(CodeBlock.BEGIN_BLOCK) > 0)
                                keyword = trimLine.Substring(0, trimLine.IndexOf(CodeBlock.BEGIN_BLOCK) + 1);
                            else
                                keyword = trimLine;
                        }
                    }

                    keywords.Add(keyword.Trim());
                }
            }
            return keywords;
        }
    }
}