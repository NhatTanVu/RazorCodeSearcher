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
OutputFilePathsMap = new Dictionary<string, string>()
{
  { @"C:\search\path\solution1.sln", "solution1_code_in_views.txt" },
  { @"C:\search\path\solution2.sln", "solution2_code_in_views.txt" }
};
```
2. Run it.
