using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Tweetinvi;
using Tweetinvi.Core.Interfaces.Streaminvi;

namespace LanguageProcessing
{
    public class ListWordsByFrequency
    {
        private Dictionary<string, int> mainDictionary;

        public string output
        {
            get
            {
                StringBuilder resultSb = new StringBuilder(mainDictionary.Count * 9);
                foreach (KeyValuePair<string, int> entry in mainDictionary)
                    resultSb.AppendLine(string.Format("{0} [{1}]", entry.Key, entry.Value));
                return resultSb.ToString();
            }
        }

        public ListWordsByFrequency()
        {
            mainDictionary = new Dictionary<string, int>();
        }

        /// <summary>
        ///  An enumeration of sorting options to be used.
        /// </summary>
        private enum SortOrder
        {
            Ascending, // from small to big numbers or alphabetically. 
            Descending // from big to small number or reversed alphabetical order 
        }

        // This will discard digits 
        private static char[] delimiters_no_digits = new char[] {
            '{', '}', '(', ')', '[', ']', '>', '<','-', '_', '=', '+',
            '|', '\\', ':', ';', ' ', ',', '.', '/', '?', '~', '!',
            '@', '#', '$', '%', '^', '&', '*', ' ', '\r', '\n', '\t',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        /// <summary>
        ///  Tokenizes a text into an array of words, using the improved
        ///  tokenizer with the discard-digit option.
        /// </summary>
        /// <param name="text"> the text to tokenize</param>
        private string[] Tokenize(string text)
        {
            string[] tokens = text.Split(delimiters_no_digits, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < tokens.Length; i++)
            {
                string token = tokens[i];

                // Change token only when it starts and/or ends with "'" and  
                // it has at least 2 characters. 

                if (token.Length > 1)
                {
                    if (token.StartsWith("'") && token.EndsWith("'"))
                        tokens[i] = token.Substring(1, token.Length - 2); // remove the starting and ending "'" 

                    else if (token.StartsWith("'"))
                        tokens[i] = token.Substring(1); // remove the starting "'" 

                    else if (token.EndsWith("'"))
                        tokens[i] = token.Substring(0, token.Length - 1); // remove the last "'" 
                }
            }

            return tokens;
        }

        /// <summary>
        ///  Make a string-integer dictionary out of an array of words.
        /// </summary>
        /// <param name="words"> the words out of which to make the dictionary</param>
        /// <returns> a string-integer dictionary</returns>
        private Dictionary<string, int> ToStrIntDict(string[] words)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();

            foreach (string word in words)
            {
                // if the word is in the dictionary, increment its freq. 
                if (dict.ContainsKey(word))
                {
                    dict[word]++;
                }
                // if not, add it to the dictionary and set its freq = 1 
                else
                {
                    dict.Add(word, 1);
                }
            }

            return dict;
        }

        private void UpdateMainDict(Dictionary<string, int> inDict)
        {
            foreach (KeyValuePair<string, int> kvPair in inDict)
            {
                // if the word is in the dictionary, increment its freq. 
                if (mainDictionary.ContainsKey(kvPair.Key))
                {
                    mainDictionary[kvPair.Key] += kvPair.Value;
                }
                // if not, add it to the dictionary and set its freq = 1 
                else
                {
                    mainDictionary.Add(kvPair.Key, kvPair.Value);
                }
            }
        }

        /// <summary>
        ///  Sort a string-int dictionary by its entries' values.
        /// </summary>
        /// <param name="strIntDict"> a string-int dictionary to sort</param>
        /// <param name="sortOrder"> one of the two enumerations: Ascending and Descending</param>
        /// <returns> a string-integer dictionary sorted by integer values</returns>
        private Dictionary<string, int> ListWordsByFreq(Dictionary<string, int> strIntDict, SortOrder sortOrder)
        {
            // Copy keys and values to two arrays 
            string[] words = new string[strIntDict.Keys.Count];
            strIntDict.Keys.CopyTo(words, 0);

            int[] freqs = new int[strIntDict.Values.Count];
            strIntDict.Values.CopyTo(freqs, 0);

            //Sort by freqs: it sorts the freqs array, but it also rearranges 
            //the words array's elements accordingly (not sorting) 
            Array.Sort(freqs, words);

            // If sort order is descending, reverse the sorted arrays. 
            if (sortOrder == SortOrder.Descending)
            {
                //reverse both arrays 
                Array.Reverse(freqs);
                Array.Reverse(words);
            }

            //Copy freqs and words to a new Dictionary<string, int> 
            Dictionary<string, int> dictByFreq = new Dictionary<string, int>();

            for (int i = 0; i < freqs.Length; i++)
            {
                dictByFreq.Add(words[i], freqs[i]);
            }

            return dictByFreq;
        }

        // Process input box text and display result in output box 
        public void split(string inString)
        {
            if (inString != string.Empty)
            {
                // Split text into array of words 
                string[] words = Tokenize(inString);

                if (words.Length > 0)
                {
                    // Make a string-int dictionary out of the array of words  
                    Dictionary<string, int> splitDictionary = ToStrIntDict(words);

                    UpdateMainDict(splitDictionary);
                    mainDictionary = ListWordsByFreq(mainDictionary, SortOrder.Descending);
                }
            }
        }
    }
}
