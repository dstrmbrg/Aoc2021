using System;
using System.Linq;

namespace Aoc2021.Puzzles;

internal class Day07 : Puzzle
{
    public override object PartOne() => CalculateMinFuelCost(d => d);

    public override object PartTwo() => CalculateMinFuelCost(d => (d * d + d) / 2);

    private int CalculateMinFuelCost(Func<int, int> fuelCostFunc)
    {
        var positions = Utilities.GetInput(GetType())
            .Split(",")
            .Select(int.Parse)
            .ToArray();

        var (start, count) = (positions.Min(), positions.Max() - positions.Min() + 1);
            
        return Enumerable.Range(start, count)
            .AsParallel()
            .Min(position => CalculateFuelCostAtPosition(positions, position, fuelCostFunc));
    }

    private static int CalculateFuelCostAtPosition(int[] positions, int position, Func<int, int> fuelCostFunc)
    {
        return positions
            .Select(p => Math.Abs(position - p))
            .Sum(fuelCostFunc);
    }
}