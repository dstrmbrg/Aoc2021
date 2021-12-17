using System;
using System.Linq;
using System.Text.RegularExpressions;
using WinstonPuckett.PipeExtensions;

namespace Aoc2021.Puzzles;

internal class Day17 : Puzzle
{
    public override object PartOne() =>
        GetTarget()
            .Pipe(Solve)
            .MaxHeight;

    public override object PartTwo() =>
        GetTarget()
            .Pipe(Solve)
            .Count;
    
    private static (int Count, int MaxHeight) Solve(Area target)
    {
        var count = 0;
        var maxVerticalVelocity = 0;
        var minHorizontalVelocity = CalculateMinHorizontalVelocity(target.X1);

        for (var vY = target.Y1; vY <= -2 * target.Y1; vY++) 
        for (var vX = minHorizontalVelocity; vX <= target.X2; vX++)
        for (var t = 0; t < int.MaxValue; t++)
        {
            var verticalPosition = CalculatePosition(vY, t);
            var horizontalPosition = CalculateHorizontalPosition(vX, t);

            if (verticalPosition.IsBetween(target.Y1, target.Y2) && horizontalPosition.IsBetween(target.X1, target.X2))
            {
                maxVerticalVelocity = vY;
                count += 1;
                break;
            }

            if (verticalPosition < target.Y1)
                break;
        }

        return (count, CalculateMaxPosition(maxVerticalVelocity));
    }

    private static int CalculateMaxPosition(int v) => v * (v + 1) / 2;
    private static int CalculatePosition(int v, int t) => (t + 1) * (2 * v - t) / 2;
    private static int CalculateHorizontalPosition(int v, int t) => t >= v ? CalculateMaxPosition(v) : CalculatePosition(v, t);
    private static int CalculateMinHorizontalVelocity(int m) => (int)Math.Ceiling((Math.Sqrt(8 * m + 1) - 1) / 2);

    private Area GetTarget()
    {
        var values = Utilities.GetInput(GetType())
            .Pipe(x => Regex.Matches(x, @"-?\d+")
                .Select(y => int.Parse(y.Value))).ToArray();

        return new Area(values[0], values[1], values[2], values[3]);
    }

    private record Area(int X1, int X2, int Y1, int Y2);
}