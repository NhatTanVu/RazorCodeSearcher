using System;
using System.Collections.Generic;
using System.Linq;

namespace RazorCodeSearcher.Models
{
    public class CodeBlock
    {
        public const string BEGIN_BLOCK = "{";
        public const string BEGIN_BLOCK_2_0 = "@{";
        public const string END_BLOCK = "}";
        public const string BEGIN_COMMENT_BLOCK = "@*";
        public const string END_COMMENT_BLOCK = "*@";
        public const string LINE_FORMATTER = "Line {0}: {1}";
        public const string BEGIN_BLOCK_SEPARATOR = "------------------------------------";
        public const string ELLIPSIS = "[...]";
        public const char ASTERISK = '$';
        public const string CODE_GENERAL = "@";
        public const string BEGIN_GROUP_SEPARATOR = "<<------BEGIN GROUP------>>";
        public const string END_GROUP_SEPARATOR = "<<------END GROUP------>>";

        private string normalizedString = string.Empty;

        public string[] Keywords { get; set; }
        public List<KeyValuePair<string, string>> Content { get; set; }
        public virtual string FilePath { get; set; }
        public virtual int StartLine { get; set; }
        public virtual int EndLine { get; set; }
        public virtual bool HasContent
        {
            get
            {
                return (Content.Count > 0) &&
                    string.Join("", Content.Select(c => c.Value)).Replace(" ", string.Empty).Replace("\t", string.Empty) != string.Join("", Keywords).Replace(" ", string.Empty).Replace("\t", string.Empty);
            }
        }
        public string NormalizedString
        {
            get
            {
                if (string.IsNullOrEmpty(normalizedString))
                    NormalizeString();
                return normalizedString;
            }
        }

        public CodeBlock(string[] keywords, string filePath)
        {
            Keywords = keywords;
            FilePath = filePath;
            Content = new List<KeyValuePair<string, string>>();
        }

        public override string ToString()
        {
            if (HasContent)
                return string.Join(Environment.NewLine, new string[]
                {
                    BEGIN_BLOCK_SEPARATOR,
                    FilePath,
                    BEGIN_BLOCK_SEPARATOR,
                    string.Join(Environment.NewLine, Content.Select(c => string.Format(LINE_FORMATTER, c.Key, c.Value)))
                });
            else
                return string.Empty;
        }

        public void NormalizeString()
        {
            if (HasContent)
                this.normalizedString = string.Join(CodeBlock.ELLIPSIS, Content.Select(c => c.Value).Where(c => !string.IsNullOrEmpty(c.Replace(" ", string.Empty).Replace("\t", string.Empty))));
            else
                this.normalizedString = string.Empty;
        }
    }
}
