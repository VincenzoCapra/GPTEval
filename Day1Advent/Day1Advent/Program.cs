using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        string filePath = @"C:\Users\vcapr\Documents\Github\GPTEval\Day1Advent\Day1Advent\input.txt";

        try
        {
            string[] lines = File.ReadAllLines(filePath);
            int validLinesCount = 0;

            for (int i = 0; i < lines.Length; i++)
            {
                bool isValidGame = CheckColorCounts(lines[i]);
                Console.WriteLine($"Game {i + 1}: {(isValidGame ? "Valid" : "Invalid")}");

                if (isValidGame)
                {
                    validLinesCount++;
                }
            }

            Console.WriteLine($"Total lines meeting the criteria: {validLinesCount}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    static bool CheckColorCounts(string game)
    {
        int redCount = 12;
        int greenCount = 13;
        int blueCount = 14;

        var counts = Regex.Matches(game, @"\d+").Cast<Match>().Select(m => int.Parse(m.Value)).ToList();

        int redOccurrences = counts.Count(c => c == redCount);
        int greenOccurrences = counts.Count(c => c == greenCount);
        int blueOccurrences = counts.Count(c => c == blueCount);

        return redOccurrences == 1 && greenOccurrences == 1 && blueOccurrences == 1;
    }
}