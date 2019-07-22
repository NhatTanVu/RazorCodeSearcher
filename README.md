# Razor Code Searcher
List out all inline code in all Razor views within C# solution.
# How to use
1. Go to **Program.cs** and change these lines:
```
private const string SEARCH_PATH = @"C:\search\path";
private const string OUTPUT_FOLDER_PATH = @"C:\output\path";
...
private const string REMAINING_OUTPUT_FILE = "remaining_code_in_views.txt";
...
OutputSolutions = new List<Solution>()
{
   new Solution {
      Name ="Solution 1", SolutionFilePath=@"C:\path\to\solution1.sln", 
      OutputFilePath = "solution1_code_in_views.txt", IncludedFolderPaths = new List<string>{}
   },
   new Solution {
      Name ="Solution 2", SolutionFilePath=@"C:\path\to\solution2.sln", 
      OutputFilePath = "solution2_code_in_views.txt", IncludedFolderPaths = new List<string>{}
   }
};
```
2. Run it.
