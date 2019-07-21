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
        public const string LINE_FORMATTER = "Line {0}: {1}";
        public const string BEGIN_BLOCK_SEPARATOR = "------------------------------------";

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

        public CodeBlock()
        {
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
    }
}
