using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Aoc2021.Puzzles;

namespace Aoc2021
{
    internal static class Program
    {
        private static void Main()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var puzzleInstances = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.BaseType == typeof(Puzzle))
                .Select(Activator.CreateInstance)
                .Select(x => (Puzzle)x)
                .ToList();

            var partOneResults = puzzleInstances
                .AsParallel()
                .Select(x => (Day: $"{x.GetType().Name}.PartOne", Result: x.PartOne()));

            var partTwoResults = puzzleInstances
                .AsParallel()
                .Select(x => (Day: $"{x.GetType().Name}.PartTwo", result: x.PartTwo()));

            var results = partOneResults
                .Concat(partTwoResults)
                .OrderBy(x => x.Day);

            foreach (var (day, result) in results)
            {
                Console.WriteLine($"{day}: {result}");
            }

            Console.WriteLine($"Elapsed Time: {stopwatch.Elapsed.TotalSeconds} seconds");
        }
    }
}
