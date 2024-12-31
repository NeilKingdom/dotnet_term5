/*
 * Program: Lab 1
 * Professor:
 * Author: Neil Kingdom
 * Student Number: 040967309
 * Description: This program performs a variety of tasks such as:
 *    - Imports words from file
 *    - Performs bubble sort on the list of words
 *    - Performs LINQ/Lambda sort on the list of words
 *    - Does a word count
 *    - Retrieves and displays count of words that begin with the letter 'j'
 *    - Retrieves and displays count of words that end with the letter 'd'
 *    - Retrieves and displays count of words that are > 4 characters long
 *    - Retrieves and displays count of words that are less than 3 characters long and start with the letter 'a'
 *    - Exits
 */

using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;

namespace Lab1_Namespace
{
   class Lab1
   {
      static string fName = @"Words.txt";
      static IList<string> wordList = new List<string>();
      static ConsoleKeyInfo keyInfo;

      static void Main(string[] args)
      {
         Lab1 obj = new Lab1();

         while (true)
         {
            Console.WriteLine("Select one of the following:\n");
            Console.WriteLine("1 - Import Words from File");
            Console.WriteLine("2 - Bubble sort words");
            Console.WriteLine("3 - LINQ/Lambda sort words");
            Console.WriteLine("4 - Count the distinct words");
            Console.WriteLine("5 - Take the first 10 words");
            Console.WriteLine("6 - Get and display words that start with 'j' and display the count");
            Console.WriteLine("7 - Get and display words that end with 'd' and display the count");
            Console.WriteLine("8 - Get and display words that are greater than 4 characters long, and display the count");
            Console.WriteLine("9 - Get and display words that are less than 3 characters long and start with the letter 'a', and display the count");
            Console.WriteLine("x - Exit\n");

            keyInfo = Console.ReadKey(true);

            switch (keyInfo.Key)
            {
               case ConsoleKey.D1: obj.ImportWords(wordList, fName); break;
               case ConsoleKey.D2: obj.BubbleSort(wordList); break;
               case ConsoleKey.D3: obj.LINQSort(wordList); break;
               case ConsoleKey.D4: obj.CountUnique(wordList); break;
               case ConsoleKey.D5: obj.GetFirstTen(wordList); break;
               case ConsoleKey.D6: obj.DispJ(wordList); break;
               case ConsoleKey.D7: obj.DispD(wordList); break;
               case ConsoleKey.D8: obj.DispGT4(wordList); break;
               case ConsoleKey.D9: obj.DispLT3(wordList); break;
               case ConsoleKey.X:  obj.Exit(); break;
               default: Console.WriteLine("Invalid input\n"); break;
            }
         }
      }

      /*
       * Function: ImportWords
       * Brief: Imports a list of words from a given text file
       */
      void ImportWords(IList<string> words, string fName)
      {
         // Clear so that we don't append list twice
         words.Clear();

         try
         {
            using (StreamReader sr = new StreamReader(fName))
            {
               string? word;
               // Note: This assumes that each word exists on its own line
               while ((word = sr.ReadLine()) != null)
               {
                  words.Add(word);
               }
               sr.Close();
            }
         }
         catch (Exception e)
         {
            Console.WriteLine("Could not find file " + fName + "\n");
            Console.WriteLine(e.Message);
            Environment.Exit(0);
         }

         Console.WriteLine("Number of words: " + words.Count() + "\n");
      }

      /*
       * Function: BubbleSort
       * Brief: Sorts words from file alphabetically using bubble sort
       */
      IList<string> BubbleSort(IList<string> words)
      {
         // Check if list is empty before proceeding
         if (words.Count() == 0)
         {
            Console.WriteLine("No words have been loaded from file...\n");
            return new List<string>();
         }

         string temp;
         var wordsCopy = new List<string>(words);

         Stopwatch stopwatch = new Stopwatch();
         stopwatch.Start();

         // Bubble sort
         for (int j = 0; j < wordsCopy.Count - 1; j++)
         {
            for (int i = 0; i < wordsCopy.Count - 1; i++)
            {
               if (string.Compare(wordsCopy[i], wordsCopy[i+1]) > 0)
               {
                  temp = wordsCopy[i+1];
                  wordsCopy[i+1] = wordsCopy[i];
                  wordsCopy[i] = temp;
               }
            }
         }

         stopwatch.Stop();
         TimeSpan stopwatchElapsed = stopwatch.Elapsed;

         Console.WriteLine("Words sorted using bubble sort:");
         foreach (string word in wordsCopy)
                Console.WriteLine(word);

         Console.WriteLine("Bubble sort took: " + Convert.ToInt32(stopwatchElapsed.TotalMilliseconds) + " ms\n");

         // TODO: Do something with ret val
         return words;
      }

      /*
       * Function: LINQSort
       * Brief: Sorts words from file alphabetically using LINQ sort
       */
      IList<string> LINQSort(IList<string> words)
      {
         // Check if list is empty before proceeding
         if (words.Count() == 0)
         {
            Console.WriteLine("No words have been loaded from file...\n");
            return new List<string>();
         }

         var wordsCopy = new List<string>(words);

         Stopwatch stopwatch = new Stopwatch();
         stopwatch.Start();

         // Order by word size, then alphabetically
         var sortWords = wordsCopy.OrderBy(word => wordsCopy.Count).ThenBy(word => word).ToList();

         stopwatch.Stop();
         TimeSpan stopwatchElapsed = stopwatch.Elapsed;

         Console.WriteLine("Words sorted using LINQ sort:");
         foreach (string word in sortWords)
                Console.WriteLine(word);

         Console.WriteLine("LINQ sort took: " + Convert.ToInt32(stopwatchElapsed.TotalMilliseconds) + " ms\n");

         // TODO: Do something with ret val
         return words;
      }

      /*
       * Function: CountUnique
       * Brief: Counts number of unique words
       */
      void CountUnique(IList<string> words)
      {
         // Check if list is empty before proceeding
         if (words.Count() == 0)
         {
            Console.WriteLine("No words have been loaded from file...\n");
            return;
         }

         var uniqueWords = words.Distinct().ToList();
         Console.WriteLine("Number of unique words: " + uniqueWords.Count() + "\n");
      }

      /*
       * Function: GetFirstTen
       * Brief: Get the first 10 items in the list
       */
      void GetFirstTen(IList<string> words)
      {
         // Check if list is empty before proceeding
         if (words.Count() == 0)
         {
            Console.WriteLine("No words have been loaded from file...\n");
            return;
         }

         var firstTen = words.Take(10).ToList();
         Console.WriteLine("First ten words are:");
         foreach (string word in firstTen)
            Console.WriteLine(word);
         Console.WriteLine();
      }

      /*
       * Function: DispJ
       * Brief: Display all words that begin with the letter 'j'
       */
      void DispJ(IList<string> words)
      {
         // Check if list is empty before proceeding
         if (words.Count() == 0)
         {
            Console.WriteLine("No words have been loaded from file...\n");
            return;
         }

         var dispj = words.Where(word => word.StartsWith('j')).ToList();
         Console.WriteLine("Words that start with \'j\':");
         foreach (string word in dispj)
            Console.WriteLine(word);
         Console.WriteLine("Number of words that start with \'j\': " + dispj.Count + "\n");
      }

      /*
       * Function: DispD
       * Brief: Display all words that end with the letter 'd'
       */
      void DispD(IList<string> words)
      {
         // Check if list is empty before proceeding
         if (words.Count() == 0)
         {
            Console.WriteLine("No words have been loaded from file...\n");
            return;
         }

         var dispd = words.Where(word => word.EndsWith('d')).ToList();
         Console.WriteLine("Words that end with \'d\':");
         foreach (string word in dispd)
            Console.WriteLine(word);
         Console.WriteLine("Number of words that end with \'d\': " + dispd.Count + "\n");
      }

      /*
       * Function: DispGT4
       * Brief: Display words that have more than 4 characters
       */
      void DispGT4(IList<string> words)
      {
         // Check if list is empty before proceeding
         if (words.Count() == 0)
         {
            Console.WriteLine("No words have been loaded from file...\n");
            return;
         }

         var gt4 = words.Where(word => word.Length > 4).ToList();
         Console.WriteLine("Words that are greater than 4 letters long:");
         foreach (string word in gt4)
            Console.WriteLine(word);
         Console.WriteLine("Number of words that have more than 4 characters: " + gt4.Count + "\n");
      }

      /*
       * Function: DispLT3
       * Brief: Display words with less than 3 characters and that start with the letter 'a'
       */
      void DispLT3(IList<string> words)
      {
         // Check if list is empty before proceeding
         if (words.Count() == 0)
         {
            Console.WriteLine("No words have been loaded from file...\n");
            return;
         }

         var lt3 = words.Where(word => word.StartsWith('a') && word.Length < 3).ToList();
         Console.WriteLine("Words that are less than 3 letters long:");
         foreach (string word in lt3)
            Console.WriteLine(word);
         Console.WriteLine("Number of words that have less than 3 characters: " + lt3.Count + "\n");
      }

      /*
       * Function: Exit
       * Brief: Exists the program
       */
      void Exit()
      {
         Console.WriteLine("Goodbye!\n");
         Environment.Exit(0);
      }
   }
}
