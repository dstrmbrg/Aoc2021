using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Aoc2021.Puzzles
{
    internal class Day08 : Puzzle
    {
        public override object PartOne()
        {
            return ReadInput()
                .SelectMany(x => x.Output)
                .Count(x => new[] { 2, 3, 4, 7 }.Contains(x.Length));
        }

        public override object PartTwo()
        {
            return ReadInput()
                .Select(x => (x.Output, Map: DecipherSignalPatterns(x.Input)))
                .Sum(x => ConvertOutputSignalsToValue(x.Output, x.Map));
        }

        private static int ConvertOutputSignalsToValue(string[] output, IDictionary<int, string> map)
        {
            return output.Select(p => GetOutputDigit(map, p))
                .Select((x, i) => x * (int)Math.Pow(10, 3 - i))
                .Sum();
        }

        private static int GetOutputDigit(IDictionary<int, string> map, string pattern)
        {
            return map.Single(m => m.Value.ToArray().ElementsEqual(pattern.ToArray())).Key;
        }

        private static IDictionary<int, string> DecipherSignalPatterns(string[] patterns)
        {
            var map = new Dictionary<int, string>
            {
                { 1, patterns.Single(p => p.Length == 2) },
                { 4, patterns.Single(p => p.Length == 4) },
                { 7, patterns.Single(p => p.Length == 3) },
                { 8, patterns.Single(p => p.Length == 7) }
            };

            map.Add(3, patterns.Single(p => p.Length == 5 && map[7].All(p.Contains)));
            map.Add(9, patterns.Single(p => p.Length == 6 && map[3].All(p.Contains)));
            map.Add(0, patterns.Single(p => p.Length == 6 && map[7].All(p.Contains) && map.None(x => x.Value == p)));
            map.Add(6, patterns.Single(p => p.Length == 6 && map.None(x => x.Value == p)));
            map.Add(5, patterns.Single(p => p.Length == 5 && p.All(x => map[6].Contains(x))));
            map.Add(2, patterns.Single(p => map.None(x => x.Value == p)));

            return map;
        }

        private (string[] Input, string[] Output)[] ReadInput()
        {
            return Utilities.GetInput(GetType())
                .Split(Environment.NewLine)
                .Select(row => Regex.Matches(row, @"\w{1,7}\b").Select(x => x.Value).ToArray())
                .Select(x => (x.Take(10).ToArray(), x.TakeLast(4).ToArray()))
                .ToArray();
        }
    }
}