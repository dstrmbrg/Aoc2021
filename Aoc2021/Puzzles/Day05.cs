using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Aoc2021.Puzzles;

internal class Day05 : Puzzle
{
    public override object PartOne() => CalculateLineOverlaps(false);

    public override object PartTwo() => CalculateLineOverlaps(true);

    private int CalculateLineOverlaps(bool includeDiagonalLines)
    {
        var lines = ReadInput();

        if (!includeDiagonalLines)
        {
            lines = lines.Where(line => line.Start.X == line.End.X || line.Start.Y == line.End.Y).ToList();
        }

        return lines.SelectMany(GetLineCoordinates)
            .GroupBy(x => x)
            .Count(x => x.Count() >= 2);
    }

    private static IList<(int X, int Y)> GetLineCoordinates(((int X, int Y) Start, (int X, int Y) End) line)
    {
        var xDirection = GetDirection(line.Start.X, line.End.X);
        var yDirection = GetDirection(line.Start.Y, line.End.Y);
        var length = Math.Max(Math.Abs(line.Start.X - line.End.X), Math.Abs(line.Start.Y - line.End.Y));

        return Enumerable.Range(0, length + 1)
            .Select(i => (line.Start.X + i * xDirection, line.Start.Y + i * yDirection))
            .ToList();
    }

    private static int GetDirection(int start, int end) =>
        (end - start) switch
        {
            0 => 0,
            > 0 => 1,
            _ => -1
        };

    private IList<((int X, int Y) Start, (int X, int Y) End)> ReadInput()
    {
        return Utilities.GetInput(GetType())
            .Split(Environment.NewLine)
            .Select(row => Regex.Matches(row, @"-?\d+")
                .Select(x => int.Parse(x.Value))
                .ToArray())
            .Select(x => ((x[0], x[1]), (x[2], x[3])))
            .ToList();
    }
}