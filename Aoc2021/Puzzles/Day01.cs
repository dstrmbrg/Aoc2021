using System;
using System.Collections.Generic;
using System.Linq;

namespace Aoc2021.Puzzles;

internal class Day01 : Puzzle
{
    public override object PartOne()
    {
        return CalculateIncreasingMeasurements(1);
    }

    public override object PartTwo()
    {
        return CalculateIncreasingMeasurements(3);
    }

    private int CalculateIncreasingMeasurements(int batchMeasurements)
    {
        var measurements = GetMeasurements();

        return measurements
            .Where((measurement, index) => index >= batchMeasurements && measurement > measurements[index - batchMeasurements])
            .Count();
    }

    private List<int> GetMeasurements()
    {
        return Utilities.GetInput(GetType())
            .Split(Environment.NewLine)
            .Select(int.Parse)
            .ToList();
    }
}