using System;
using System.Collections.Generic;
using System.Linq;

namespace Aoc2021.Puzzles;

internal class Day14 : Puzzle
{
    public override object PartOne() => Solve(10);

    public override object PartTwo() => Solve(40);

    private long Solve(int steps)
    {
        var (pairCount, rules, lastCharacter) = ReadInput();

        for (var i = 0; i < steps; i++)
            foreach (var (pair, currentValue) in pairCount.ToArray())
            {
                var insert = rules[pair];
                pairCount[pair] -= currentValue;
                pairCount.AddOrIncrement(pair[..1] + insert, currentValue);
                pairCount.AddOrIncrement(insert + pair[^1..], currentValue);
            }

        var elementCount = pairCount
            .GroupBy(x => x.Key.ToCharArray().First())
            .Select(x => (Char: x.Key, Count: x.Sum(y => y.Value)))
            .ToDictionary(x => x.Char, x => x.Count);

        elementCount[lastCharacter] += 1;

        return elementCount.Max(x => x.Value) - elementCount.Min(x => x.Value);
    }

    private (IDictionary<string, long> PairCount, IDictionary<string, char> Rules, char LastCharacter) ReadInput()
    {
        var split = Utilities.GetInput(GetType())
            .Split(Environment.NewLine + Environment.NewLine);

        var rules = split[1]
            .Split(Environment.NewLine)
            .Select(x => x.Split("->"))
            .Select(x => (x[0].Trim(), x[1][1]))
            .ToDictionary(x => x.Item1, x => x.Item2);

        var pairCountDictionary = new Dictionary<string, long>();
        var start = split[0];

        for (var i = 0; i < start.Length - 1; i++) 
            pairCountDictionary.AddOrIncrement(start.Substring(i, 2), 1);

        return (pairCountDictionary, rules, start.Last());
    }
}