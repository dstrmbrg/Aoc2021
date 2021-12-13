using System;
using System.Collections.Generic;
using System.Linq;

namespace Aoc2021.Puzzles;

internal class Day13 : Puzzle
{
    public override object PartOne()
    {
        var (dots, folds) = ReadInput();

        return Fold(dots, folds.Take(1).ToList()).Count;
    }

    public override object PartTwo()
    {
        var (dots, folds) = ReadInput();

        var result = Fold(dots, folds);

        return GetDotsPrintableString(result);
    }

    private static IList<Dot> Fold(IList<Dot> dots, IList<(char Axis, int Coordinate)> folds)
    {
        foreach (var (axis, coordinate) in folds)
        {
            var dotsToReflect = axis == 'x'
                ? dots.Where(d => d.X > coordinate).ToList()
                : dots.Where(d => d.Y > coordinate).ToList();

            foreach (var dotToReflect in dotsToReflect)
            {
                if (axis == 'x')
                {
                    dotToReflect.X = Reflect(dotToReflect.X, coordinate);
                }
                else
                {
                    dotToReflect.Y = Reflect(dotToReflect.Y, coordinate);
                }
            }
        }

        return dots.DistinctBy(d => (d.X, d.Y)).ToList();
    }

    private static int Reflect(int position, int reflectAt) => 2 * reflectAt - position;

    private static string GetDotsPrintableString(IList<Dot> dots)
    {
        var xMax = dots.Max(d => d.X);
        var yMax = dots.Max(d => d.Y);

        var dotsDict = dots.ToDictionary(d => (d.X, d.Y));

        var result = Environment.NewLine;

        for (var y = 0; y <= yMax; y++)
        {
            for (var x = 0; x <= xMax; x++)
            {
                result += dotsDict.ContainsKey((x, y)) ? "#" : ".";
            }

            result += Environment.NewLine;
        }

        return result;
    }

    private (IList<Dot> Dots, IList<(char Axis, int Coordinate)> Folds) ReadInput()
    {
        var split = Utilities.GetInput(GetType()).Split(Environment.NewLine + Environment.NewLine);

        var dots = split[0]
            .Split(Environment.NewLine)
            .Select(x => x.Split(",").ToArray())
            .Select(x => new Dot()
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