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
        private const string DISTANCE_OUTPUT_FILE = "distances.txt";
        private const string CODE_OUTPUT_FILE = "code_{0}.txt";
        private const string SIMILAR_FOLDER = "Similar";
        private const string REMAINING_FOLDER = "Remaining_code";
        private const string FILE_EXT = ".txt";
        private const SimilarityLevel MAX_LEVEL = SimilarityLevel.Medium;
        private static readonly string[] ExcludedPaths;
        private static readonly string[][] ListOfKeywords;
        private static readonly List<Solution> OutputSolutions;
        

        static Program()
        {
            ListOfKeywords = new string[][] {
                new string[] { CodeBlock.ELLIPSIS + CodeBlock.BEGIN_COMMENT_BLOCK },
                new string[] { CodeBlock.ELLIPSIS + "@functions" },
                
                new string[] { CodeBlock.ELLIPSIS + "@Html.Raw", CodeBlock.ELLIPSIS + "@Model.", CodeBlock.ELLIPSIS + "@Html.Raw" },
                new string[] { CodeBlock.ELLIPSIS + "@foreach" },
                new string[] { CodeBlock.ELLIPSIS + "@if", CodeBlock.ELLIPSIS + "else" + CodeBlock.ASTERISK },
                new string[] { CodeBlock.ELLIPSIS + "@for" },
                new string[] { CodeBlock.ELLIPSIS + "@" }

                //new string[] { CodeBlock.ELLIPSIS + "@Html.Raw" }, //REMOVED
                //new string[] { CodeBlock.ELLIPSIS + "@(" }, //REMOVED
                //new string[] { CodeBlock.ELLIPSIS + "@:" }, //REMOVED
                //new string[] { CodeBlock.ELLIPSIS + "@ViewBag" }, //REMOVED
                //new string[] { CodeBlock.ELLIPSIS + "@do" }, //REMOVED
            };
            ExcludedPaths = new string[] { @"\obj\", @"\bin\", @"\serialization" };
            OutputSolutions = new List<Solution>()
            {
                new Solution {
                    Name ="Solution 1", SolutionFilePath=@"C:\path\to\solution1.sln", OutputFolder = "Solution 1", IncludedFolderPaths = new List<string>{}
                },
                new Solution {
                    Name ="Solution 2", SolutionFilePath=@"C:\path\to\solution2.sln", OutputFolder = "Solution 2", IncludedFolderPaths = new List<string>{}
                }
            };
        }

        private static List<KeywordsSearchResult> SearchCodeBlocks(string searchPath, string filter, string[][] listOfKeywords, 
            string[] excludedPaths, string outputFolder, List<string> projectFolderPaths = null)
        {
            Directory.CreateDirectory(Path.Combine(OUTPUT_FOLDER_PATH, outputFolder));

            List<KeywordsSearchResult> outputBlocks = new List<KeywordsSearchResult>();
            string[] searchFiles = Directory.GetFiles(searchPath, filter, SearchOption.AllDirectories);
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
                var keywordSearchResult = new KeywordsSearchResult(keywords, blocksList.ToArray());
                string outputFilePath = Path.Combine(OUTPUT_FOLDER_PATH, outputFolder, string.Format(CODE_OUTPUT_FILE, keywordSearchResult.GetFileName()));
                File.WriteAllText(outputFilePath, keywordSearchResult.ToString());
                outputBlocks.Add(keywordSearchResult);
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

        private static void SearchSimilarCodeBlockPairs(List<KeywordsSearchResult> searchResults, string outputFolder)
        {
            Directory.CreateDirectory(Path.Combine(OUTPUT_FOLDER_PATH, outputFolder));
            Directory.CreateDirectory(Path.Combine(OUTPUT_FOLDER_PATH, outputFolder, SIMILAR_FOLDER));

            File.WriteAllLines(Path.Combine(OUTPUT_FOLDER_PATH, outputFolder, DISTANCE_OUTPUT_FILE), searchResults.Select(res => res.DistanceMatrix.ToString()));

            List<KeyValuePair<ComplexCodeBlock, ComplexCodeBlock>> similarCodeBlocks = new List<KeyValuePair<ComplexCodeBlock, ComplexCodeBlock>>();
            foreach (var searchResult in searchResults)
            {
                if (searchResult.ListOfKeywords.Length == 1 && searchResult.ListOfKeywords[0] == CodeBlock.ELLIPSIS + CodeBlock.BEGIN_COMMENT_BLOCK)
                    continue;
                string outputFileName = string.Format(CODE_OUTPUT_FILE, searchResult.GetFileName() + SearchResult.FILENAME_SEPARATOR + MAX_LEVEL.ToString());
                string outputFilePath = Path.Combine(OUTPUT_FOLDER_PATH, outputFolder, SIMILAR_FOLDER, outputFileName);
                var codeblockGroups = searchResult.DistanceMatrix.GetSimilarCodeBlocks(MAX_LEVEL);
                if (codeblockGroups.Count > 0)
                    File.WriteAllLines(outputFilePath, codeblockGroups
                        .Select(group => CodeBlock.BEGIN_GROUP_SEPARATOR + Environment.NewLine + string.Join(Environment.NewLine, group.Select(block => block.ToString()))
                            + Environment.NewLine + CodeBlock.END_GROUP_SEPARATOR));
            }
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
                string outputFolder = solution.OutputFolder;
                excludedFolderPaths.AddRange(projectFolderPaths);
                List<KeywordsSearchResult> searchResult = SearchCodeBlocks(SEARCH_PATH, SEARCH_FILTER, ListOfKeywords, ExcludedPaths, outputFolder, projectFolderPaths);
                SearchSimilarCodeBlockPairs(searchResult, outputFolder);
            }
            List<KeywordsSearchResult> remainingSearchResults = SearchCodeBlocks(SEARCH_PATH, SEARCH_FILTER, ListOfKeywords, excludedFolderPaths.ToArray(), REMAINING_FOLDER);
            SearchSimilarCodeBlockPairs(remainingSearchResults, REMAINING_FOLDER);
        }
    }
}