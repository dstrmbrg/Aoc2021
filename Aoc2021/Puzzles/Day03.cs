using System;
using System.Collections.Generic;
using System.Linq;

namespace Aoc2021.Puzzles;

internal class Day03 : Puzzle
{
    public override object PartOne()
    {
        var input = Utilities.GetInput(GetType());
        var numbers = input.Split(Environment.NewLine);
        var noRows = numbers.Length;
        var accumulator = new int[numbers[0].Length];

        foreach (var number in numbers)
        {
            var bits = number.ToCharArray().Select(x => int.Parse(x.ToString())).ToArray();

            for (var i = 0; i < bits.Length; i++)
            {
                accumulator[i] += bits[i];
            }
        }

        var gamma = Convert.ToInt32(string.Join(string.Empty, accumulator.Select(x => (x > noRows / 2) ? "1" : "0").ToArray()), 2);
        var epsilon = Convert.ToInt32(string.Join(string.Empty, accumulator.Select(x => (x < noRows / 2) ? "1" : "0").ToArray()), 2);

        return gamma * epsilon;
    }

    public override object PartTwo()
    {
        var input = Utilities.GetInput(GetType());
        var numbers = input.Split(Environment.NewLine).ToList();

        var oxygenGeneratorRating = GetRating(numbers.ToList(), true);
        var scrubberRating = GetRating(numbers.ToList(), false);

        return oxygenGeneratorRating * scrubberRating;
    }

    private static int GetRating(List<string> numbers, bool keepMostCommon)
    {
        char GetBit(int sum)
        {
            var mostOnes = sum >= (numbers.Count + 1) / 2;

            if (keepMostCommon)
            {
                return mostOnes ? '1' : '0';
            }

            return mostOnes ? '0' : '1';
        }

        for (var i = 0; i < numbers[0].Length; i++)
        {
            var sum = numbers.Sum(x => int.Parse(x[i].ToString()));
            var bit = GetBit(sum);
            numbers.RemoveAll(x => x[i] != bit);

            if (numbers.Count == 1)
            {
                break;
            }
        }

        return Convert.ToInt32(numbers[0], 2);
    }

}