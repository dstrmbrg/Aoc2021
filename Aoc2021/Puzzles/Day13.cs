using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinstonPuckett.PipeExtensions;

namespace Aoc2021.Puzzles;

internal class Day13 : Puzzle
{
    public override object PartOne() => ReadInput()
            .Pipe(x => FoldDots(x.Dots, x.Folds.Take(1)))
            .Count;

    public override object PartTwo() => ReadInput()
            .Pipe(FoldDots)
            .Pipe(ToPrintableString);

    private static IList<Dot> FoldDots(IEnumerable<Dot> dots, IEnumerable<(char Axis, int Coordinate)> folds)
    {
        return dots.Select(dot => FoldDot(dot, folds))
            .DistinctBy(d => (d.X, d.Y))
            .ToList();
    }

    private static Dot FoldDot(Dot dot, IEnumerable<(char Axis, int Coordinate)> folds)
    {
        foreach (var (axis, coordinate) in folds)
        {
            switch (axis)
            {
                case 'x' when dot.X < coordinate:
                case 'y' when dot.Y < coordinate:
                    continue;
                case 'x':
                    dot.X = Reflect(dot.X, coordinate);
                    break;
                case 'y':
                    dot.Y = Reflect(dot.Y, coordinate);
                    break;
            }
        }

        return dot;
    }

    private static int Reflect(int position, int reflectAt) => 2 * reflectAt - position;

    private static string ToPrintableString(IList<Dot> dots)
    {
        var xMax = dots.Max(d => d.X);
        var yMax = dots.Max(d => d.Y);
        var dotsDict = dots.ToDictionary(d => (d.X, d.Y));
        var sb = new StringBuilder(Environment.NewLine);

        for (var y = 0; y <= yMax; y++)
        {
            for (var x = 0; x <= xMax; x++) 
                sb.Append(dotsDict.ContainsKey((x, y)) ? "#" : ".");

            sb.Append(Environment.NewLine);
        }

        return sb.ToString();
    }

    private (IList<Dot> Dots, IList<(char Axis, int Coordinate)> Folds) ReadInput()
    {
        var split = Utilities.GetInput(GetType()).Split(Environment.NewLine + Environment.NewLine);

        var dots = split[0]
            .Split(Environment.NewLine)
            .Select(x => x.Split(",").ToArray())
            .Select(x => new Dot
            {
                X = int.Parse(x[0]),
                Y = int.Parse(x[1])
            })
            .ToList();

        var folds = split[1]
            .Split(Environment.NewLine)
            .Select(x => x.Split("=").ToArray())
            .Select(x => (x[0].Last(), int.Parse(x[1])))
            .ToList();

        return (dots, folds);
    }

    private class Dot
    {
        public int X;
        public int Y;
    }
}