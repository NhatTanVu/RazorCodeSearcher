using RazorCodeSearcher.Finders;
using RazorCodeSearcher.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RazorCodeSearcher
{
    class Program
    {
        private const string SEARCH_FILTER = "*.cshtml";
        private const string SEARCH_PATH = @"C:\search\path";
        private const string OUTPUT_FOLDER_PATH = @"C:\output\path";
        private const string REMAINING_OUTPUT_FILE = "remaining_code_in_views.txt";
        private const string KEYWORD_OUTPUT_FILE = "keywords.txt";
        private static readonly string[] ExcludedPaths;
        private static readonly string[][] ListOfKeywords;
        private static readonly List<Solution> OutputSolutions;

        static Program()
        {
            ListOfKeywords = new string[][] {
                //new string[] { CodeBlock.ELLIPSIS + "@functions" },
                //new string[] { CodeBlock.ELLIPSIS + "@Html.Raw", CodeBlock.ELLIPSIS + "@Model.", CodeBlock.ELLIPSIS + "@Html.Raw" },
                //new string[] { CodeBlock.ELLIPSIS + "@Html.Raw" },
                //new string[] { CodeBlock.ELLIPSIS + "@*" },
                //new string[] { CodeBlock.ELLIPSIS + "@foreach" },
                //new string[] { CodeBlock.ELLIPSIS + "@if" },
                //new string[] { CodeBlock.ELLIPSIS + "@for" },
                //new string[] { CodeBlock.ELLIPSIS + "@(" }, //REMOVED
                //new string[] { CodeBlock.ELLIPSIS + "@:" }, //REMOVED
                //new string[] { CodeBlock.ELLIPSIS + "@ViewBag" }, //REMOVED
                //new string[] { CodeBlock.ELLIPSIS + "@do" }, //REMOVED
                //new string[] { CodeBlock.ELLIPSIS + "@" }
            };
            ExcludedPaths = new string[] { @"\obj\", @"\bin\", @"\serialization" };
            OutputSolutions = new List<Solution>()
            {
                new Solution {
                    Name ="Solution 1", SolutionFilePath=@"C:\path\to\solution1.sln", OutputFilePath = "solution1_code_in_views.txt", IncludedFolderPaths = new List<string>{}
                },
                new Solution {
                    Name ="Solution 2", SolutionFilePath=@"C:\path\to\solution2.sln", OutputFilePath = "solution2_code_in_views.txt", IncludedFolderPaths = new List<string>{}
                }
            };
        }

        private static List<KeywordsSearchResult> SearchCodeBlocks(string path, string filter, string[][] listOfKeywords, string[] excludedPaths, List<string> projectFolderPaths = null)
        {
            List<KeywordsSearchResult> outputBlocks = new List<KeywordsSearchResult>();
            string[] searchFiles = Directory.GetFiles(path, filter, SearchOption.AllDirectories);
            ComplexCodeBlockFinder finder = new ComplexCodeBlockFinder();
            foreach (string[] keywords in listOfKeywords)
            {
                List<ComplexCodeBlock> blocksList = new List<ComplexCodeBlock>();
                foreach (string file in searchFiles)
                {
                    if (!excludedPaths.Any(p => file.IndexOf(p, StringComparison.OrdinalIgnoreCase) >= 0) &&
                        (projectFolderPaths == null || projectFolderPaths.Any(p => file.IndexOf(p, StringComparison.OrdinalIgnoreCase) >= 0)))
                    {
                        var lines = File.ReadLines(file).ToList();
                        ComplexCodeBlock block;
                        if (finder.FindAll(keywords, file, lines, out block))
                        {
                            blocksList.Add(block);
                        }
                    }
                }
                outputBlocks.Add(new KeywordsSearchResult(keywords, blocksList.ToArray()));
            }
            return outputBlocks;
        }

        private static List<string> GetProjectFolderPaths(string solutionFilePath)
        {
            string projectFileExtension = ".csproj";
            string lineSeparator = ", ";
            List<string> lines = File.ReadLines(solutionFilePath).ToList();
            List<string> folderPaths = new List<string>();
            string currentWorkingFolder = Directory.GetCurrentDirectory();
            foreach (string line in lines)
            {
                var parts = line.Split(new string[] { lineSeparator }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 3 && parts[1].IndexOf(projectFileExtension) > 0)
                {
                    try
                    {
                        Directory.SetCurrentDirectory(Path.GetDirectoryName(solutionFilePath));
                        folderPaths.Add(new FileInfo(parts[1].Replace("\"", "")).Directory.FullName);
                    }
                    catch { }
                }
            }
            Directory.SetCurrentDirectory(currentWorkingFolder);
            return folderPaths;
        }

        private static List<string> SearchKeywords(string path, string filter, string[] excludedPaths)
        {
            List<string> keywords = new List<string>();
            string[] searchFiles = Directory.GetFiles(path, filter, SearchOption.AllDirectories);
            KeywordFinder finder = new KeywordFinder();
            foreach (string file in searchFiles)
            {
                if (!excludedPaths.Any(p => file.IndexOf(p, StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    var lines = File.ReadLines(file).ToList();
                    keywords = keywords.Union(finder.FindAll(file, lines)).ToList();
                }
            }
            return keywords;
        }

        static void Main(string[] args)
        {
            List<string> keywords = SearchKeywords(SEARCH_PATH, SEARCH_FILTER, ExcludedPaths);
            File.WriteAllLines(Path.Combine(OUTPUT_FOLDER_PATH, KEYWORD_OUTPUT_FILE), keywords);

            List<string> excludedFolderPaths = new List<string>(ExcludedPaths);
            foreach (var solution in OutputSolutions)
            {
                string solutionFilePath = solution.SolutionFilePath;
                List<string> projectFolderPaths = GetProjectFolderPaths(solutionFilePath);
                projectFolderPaths.AddRange(solution.IncludedFolderPaths);
                string outputPath = solution.OutputFilePath;
                excludedFolderPaths.AddRange(projectFolderPaths);
                List<KeywordsSearchResult> searchResult = SearchCodeBlocks(SEARCH_PATH, SEARCH_FILTER, ListOfKeywords, ExcludedPaths, projectFolderPaths);
                File.WriteAllLines(Path.Combine(OUTPUT_FOLDER_PATH, outputPath), searchResult.Select(res => res.ToString()));
            }
            List<KeywordsSearchResult> remainingSearchResults = SearchCodeBlocks(SEARCH_PATH, SEARCH_FILTER, ListOfKeywords, excludedFolderPaths.ToArray());
            File.WriteAllLines(Path.Combine(OUTPUT_FOLDER_PATH, REMAINING_OUTPUT_FILE), remainingSearchResults.Select(res => res.ToString()));
        }
    }
}
