using System;
using System.Collections.Generic;
using System.Linq;

namespace ImagineCommunications.GamePlan.Domain.Generic.Types
{
    /// <summary>
    /// Rules for matching a target string with a search string
    /// </summary>
    public class StringMatchRules
    {
        /// <summary>
        /// How many words must match
        /// </summary>
        public StringMatchHowManyWordsToMatch HowManyWordsToMatch { get; internal set; }

        /// <summary>
        /// Order of matching words
        /// </summary>
        public StringMatchWordOrders WordOrder { get; internal set; }

        /// <summary>
        /// How words are compared
        /// </summary>
        public StringMatchWordComparisons WordComparison { get; internal set; }

        /// <summary>
        /// Whether match is case sensitive
        /// </summary>
        public bool CaseSensitive { get; internal set; }

        /// <summary>
        /// Word delimiters
        /// </summary>
        public IEnumerable<char> WordDelimiters { get; internal set; }

        /// <summary>
        /// Characters to ignore
        /// </summary>
        public IEnumerable<char> CharactersToIgnore { get; internal set; }

        public StringMatchRules(StringMatchHowManyWordsToMatch howManyWordsToMatch, StringMatchWordOrders wordOrder,
                                    StringMatchWordComparisons wordComparison, bool caseSensitive,
                                    IEnumerable<char> wordDelimiters, IEnumerable<char> charactersToIgnore)
        {
            HowManyWordsToMatch = howManyWordsToMatch;
            WordOrder = wordOrder;
            WordComparison = wordComparison;
            CaseSensitive = caseSensitive;
            WordDelimiters = wordDelimiters;
            CharactersToIgnore = charactersToIgnore;
        }

        /// <summary>
        /// Returns whether the target string matches the search string using the rules for matching
        ///
        /// Wildcards are not currently supported. RegEx is not currently supported.
        /// </summary>
        /// <param name="targetString"></param>
        /// <param name="searchString"></param>
        /// <returns></returns>
        public bool IsMatches(string targetString, string searchString)
        {
            if (String.IsNullOrWhiteSpace(searchString))  // Empty string is a match
            {
                return true;
            }
            if (String.IsNullOrWhiteSpace(targetString) && String.IsNullOrWhiteSpace(searchString))
            {
                return true;
            }
            if (!String.IsNullOrWhiteSpace(targetString) && !String.IsNullOrWhiteSpace(searchString))
            {
                // Check if strings are identical
                if (CaseSensitive && targetString.Trim() == searchString.Trim() || !CaseSensitive && targetString.ToLower().Trim() == searchString.ToLower().Trim())
                {
                    return true;
                }

                char defaultWordDelimiter = WordDelimiters.First();    // Space?

                // Replace characters to ignore
                if (CharactersToIgnore != null && CharactersToIgnore.Any())
                {
                    foreach (var characterToIgnore in CharactersToIgnore)
                    {
                        targetString = targetString.Replace(characterToIgnore, defaultWordDelimiter);
                    }
                }

                // Replace word delimiters in target string with default word delimiter
                foreach (char wordDelimiter in WordDelimiters.Where(wd => wd != defaultWordDelimiter))
                {
                    targetString = targetString.Replace(wordDelimiter, defaultWordDelimiter);
                }

                // Check if strings are identical
                if (CaseSensitive && targetString.Trim() == searchString.Trim() || !CaseSensitive && targetString.ToLower().Trim() == searchString.ToLower().Trim())
                {
                    return true;
                }

                // Strip multiple consecutive word delimiters
                while (targetString.Contains(defaultWordDelimiter.ToString() + defaultWordDelimiter.ToString()))
                {
                    targetString = targetString.Replace(defaultWordDelimiter.ToString() + defaultWordDelimiter.ToString(), defaultWordDelimiter.ToString());
                }
                while (searchString.Contains(defaultWordDelimiter.ToString() + defaultWordDelimiter.ToString()))
                {
                    searchString = searchString.Replace(defaultWordDelimiter.ToString() + defaultWordDelimiter.ToString(), defaultWordDelimiter.ToString());
                }

                // Split input string and search string in to words
                var searchStringWords = searchString.Split(defaultWordDelimiter).Distinct().ToArray();
                var targetWords = targetString.Split(defaultWordDelimiter).Distinct().ToArray();

                // Function to match words either exact/contains/starts with/ends with
                bool isWordsMatch(string targetWord, string searchWord, StringMatchRules stringMatchRulesForWord)
                {
                    // Handle case
                    targetWord = CaseSensitive ? targetWord.Trim() : targetWord.ToLower().Trim();
                    searchWord = CaseSensitive ? searchWord.Trim() : searchWord.ToLower().Trim();

                    switch (stringMatchRulesForWord.WordComparison)
                    {
                        case StringMatchWordComparisons.ContainsWord: return targetWord.Contains(searchWord);
                        case StringMatchWordComparisons.EndsWithWord: return targetWord.EndsWith(searchWord);
                        case StringMatchWordComparisons.ExactWord: return targetWord == searchWord;
                        case StringMatchWordComparisons.StartsWithWord: return targetWord.StartsWith(searchWord);
                    }
                    return false;
                }

                // Check any word matches
                if (HowManyWordsToMatch == StringMatchHowManyWordsToMatch.AnyWord)   // Just require any word to match
                {
                    foreach (string searchTextWord in searchStringWords)
                    {
                        if (isWordsMatch(targetString, searchTextWord, this))   // Word matched
                        {
                            return true;
                        }
                    }
                    return false;
                }

                // Check all word matches in exact order
                if (HowManyWordsToMatch == StringMatchHowManyWordsToMatch.AllWords &&
                    WordOrder == StringMatchWordOrders.ExactOrder)
                {
                    int startTargetWordIndex = 0;
                    foreach (string searchTextWord in searchStringWords)
                    {
                        bool isWordMatched = false;
                        for (int targetWordIndex = startTargetWordIndex; targetWordIndex < targetWords.Length && !isWordMatched; targetWordIndex++)
                        {
                            if (isWordsMatch(targetWords[targetWordIndex], searchTextWord, this))
                            {
                                isWordMatched = true;
                                startTargetWordIndex = targetWordIndex + 1;
                            }
                        }
                        if (!isWordMatched)   // Not matched in exact order
                        {
                            return false;
                        }
                    }
                    return true;
                }

                // Check all word matches in any order. Need to be careful not to match a word twice. E.g. If target='ABCTest1 something' then we shouldn't
                // return it if they search for 'ABC 1'.
                if (HowManyWordsToMatch == StringMatchHowManyWordsToMatch.AllWords &&
                    WordOrder == StringMatchWordOrders.AnyOrder)
                {
                    // Store list of input word indexes so that we can remove matched words as we find them
                    var targetWordsLeftIndexes = new List<int>();
                    for (int index = 0; index < targetWords.ToList().Count; index++)
                    {
                        targetWordsLeftIndexes.Add(index);
                    }

                    // Check each search word exists in target word
                    foreach (string searchTextWord in searchStringWords)
                    {
                        // Check all words that match search word
                        var wordsMatchedIndex = new List<int>();
                        for (int index = 0; index < targetWordsLeftIndexes.Count; index++)
                        {
                            string targetWord = targetWords[targetWordsLeftIndexes[index]];
                            if (isWordsMatch(targetWord, searchTextWord, this))    // Word found, add to list
                            {
                                wordsMatchedIndex.Add(targetWordsLeftIndexes[index]);
                            }
                        }

                        // Check matched words
                        if (wordsMatchedIndex.Count == 0)       // Search word not found
                        {
                            return false;
                        }
                        else   // Search word found, use best match (Closest word length) if multiple words matched
                        {
                            int indexOfTargetWordToUse = -1;
                            int indexOfTargetWordThatIsBest = -1;
                            for (int index = 0; index < wordsMatchedIndex.Count; index++)
                            {
                                string targetWord = targetWords[wordsMatchedIndex[index]];
                                if (searchTextWord == targetWord)   // Exact match, use this one
                                {
                                    indexOfTargetWordToUse = wordsMatchedIndex[index];
                                    break;
                                }
                                else if (indexOfTargetWordThatIsBest == -1)    // First word that matches, set it to closest so far
                                {
                                    indexOfTargetWordThatIsBest = wordsMatchedIndex[index];
                                }
                                else if (Math.Abs(targetWord.Length - searchTextWord.Length) < Math.Abs(targetWords[indexOfTargetWordThatIsBest].Length - searchTextWord.Length))  // This word is closest match to search word so far
                                {
                                    indexOfTargetWordThatIsBest = wordsMatchedIndex[index];
                                }
                            }
                            indexOfTargetWordToUse = indexOfTargetWordToUse == -1 ? indexOfTargetWordThatIsBest : indexOfTargetWordToUse;  // Use closest length if not set
                            indexOfTargetWordToUse = indexOfTargetWordToUse == -1 ? wordsMatchedIndex[0] : indexOfTargetWordToUse;         // Default to first word matched
                            _ = targetWordsLeftIndexes.Remove(indexOfTargetWordToUse);  // Remove used word
                        }
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
