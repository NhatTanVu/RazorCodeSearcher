using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RazorCodeSearcher.Models
{
    public class Solution
    {
        public string Name { get; set; }
        public string SolutionFilePath { get; set; }
        public List<string> IncludedFolderPaths { get; set; }
        public string OutputFilePath { get; set; }

        public Solution()
        {
            IncludedFolderPaths = new List<string>();
        }
    }
}