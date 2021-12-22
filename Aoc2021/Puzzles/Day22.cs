using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WinstonPuckett.PipeExtensions;

namespace Aoc2021.Puzzles;

internal class Day22 : Puzzle
{
    public override object PartOne() => GetInstructions()
        .Pipe(FilterInitializationProcedure)
        .Pipe(Solve);
    
    public override object PartTwo() => GetInstructions()
        .Pipe(Solve);

    private static long Solve(IList<Instruction> instructions)
    {
        var cuboids = new List<Cuboid>();

        foreach (var instruction in instructions)
        {
            var newCuboids = new List<Cuboid>();
            var deletedCuboids = new List<Cuboid>();

            foreach (var cuboid in cuboids)
            {
                if (!IntersectingCuboids(cuboid, instruction.Cuboid))
                    continue;

                var splitCuboids = SplitCuboid(cuboid, instruction.Cuboid);
                newCuboids.AddRange(splitCuboids);
                deletedCuboids.Add(cuboid);
            }

            foreach (var deletedCuboid in deletedCuboids) 
                cuboids.Remove(deletedCuboid);

            cuboids.AddRange(newCuboids);
            
            if (instruction.Type == "on")
                cuboids.Add(instruction.Cuboid);
        }

        return cuboids.Sum(c => (c.X2 - c.X1 + 1) * (c.Y2 - c.Y1 + 1) * (c.Z2 - c.Z1 + 1));
    }
    
    private static IList<Cuboid> SplitCuboid(Cuboid oldCuboid, Cuboid newCuboid)
    {
        var splitCuboids = new List<Cuboid>();

        if (SplitLeft(oldCuboid, newCuboid, out var cuboid))
            splitCuboids.Add(cuboid);
        if (SplitRight(oldCuboid, newCuboid, out cuboid))
            splitCuboids.Add(cuboid);
        if (SplitTop(oldCuboid, newCuboid, out cuboid))
            splitCuboids.Add(cuboid);
        if (SplitBottom(oldCuboid, newCuboid, out cuboid))
            splitCuboids.Add(cuboid);
        if (SplitFront(oldCuboid, newCuboid, out cuboid))
            splitCuboids.Add(cuboid);
        if (SplitBack(oldCuboid, newCuboid, out cuboid))
            splitCuboids.Add(cuboid);

        return splitCuboids;
    }

    private static bool SplitLeft(Cuboid oldCuboid, Cuboid newCuboid, out Cuboid split)
    {
        split = null;

        if (oldCuboid.X1 > newCuboid.X1)
            return false;

        split = new Cuboid(oldCuboid.X1, newCuboid.X1 - 1, oldCuboid.Y1, oldCuboid.Y2, oldCuboid.Z1, oldCuboid.Z2);
        return true;
    }

    private static bool SplitRight(Cuboid oldCuboid, Cuboid newCuboid, out Cuboid split)
    {
        split = null;

        if (oldCuboid.X2 < newCuboid.X2)
            return false;

        split = new Cuboid(newCuboid.X2 + 1, oldCuboid.X2, oldCuboid.Y1, oldCuboid.Y2, oldCuboid.Z1, oldCuboid.Z2);
        return true;
    }

    private static bool SplitBottom(Cuboid oldCuboid, Cuboid newCuboid, out Cuboid split)
    {
        split = null;

        if (oldCuboid.Y1 > newCuboid.Y1)
            return false;

        var x1 = Math.Max(oldCuboid.X1, newCuboid.X1);
        var x2 = Math.Min(oldCuboid.X2, newCuboid.X2);

        split = new Cuboid(x1, x2, oldCuboid.Y1, newCuboid.Y1 - 1, oldCuboid.Z1, oldCuboid.Z2);
        return true;
    }

    private static bool SplitTop(Cuboid oldCuboid, Cuboid newCuboid, out Cuboid split)
    {
        split = null;

        if (oldCuboid.Y2 < newCuboid.Y2)
            return false;

        var x1 = Math.Max(oldCuboid.X1, newCuboid.X1);
        var x2 = Math.Min(oldCuboid.X2, newCuboid.X2);

        split = new Cuboid(x1, x2, newCuboid.Y2 + 1, oldCuboid.Y2, oldCuboid.Z1, oldCuboid.Z2);
        return true;
    }

    private static bool SplitFront(Cuboid oldCuboid, Cuboid newCuboid, out Cuboid split)
    {
        split = null;

        if (oldCuboid.Z1 > newCuboid.Z1)
            return false;

        var x1 = Math.Max(oldCuboid.X1, newCuboid.X1);
        var x2 = Math.Min(oldCuboid.X2, newCuboid.X2);
        var y1 = Math.Max(oldCuboid.Y1, newCuboid.Y1);
        var y2 = Math.Min(oldCuboid.Y2, newCuboid.Y2);

        split = new Cuboid(x1, x2, y1, y2, oldCuboid.Z1, newCuboid.Z1 - 1);
        return true;
    }

    private static bool SplitBack(Cuboid oldCuboid, Cuboid newCuboid, out Cuboid split)
    {
        split = null;

        if (oldCuboid.Z2 < newCuboid.Z2)
            return false;

        var x1 = Math.Max(oldCuboid.X1, newCuboid.X1);
        var x2 = Math.Min(oldCuboid.X2, newCuboid.X2);
        var y1 = Math.Max(oldCuboid.Y1, newCuboid.Y1);
        var y2 = Math.Min(oldCuboid.Y2, newCuboid.Y2);

        split = new Cuboid(x1, x2, y1, y2, newCuboid.Z2 + 1, oldCuboid.Z2);
        return true;
    }

    private static bool IntersectingCuboids(Cuboid first, Cuboid second)
    {
        if (first.X2 < second.X1)
            return false;
        if (first.X1 > second.X2)
            return false;
        if (first.Y2 < second.Y1)
            return false;
        if (first.Y1 > second.Y2)
            return false;
        if (first.Z2 < second.Z1)
            return false;
        return first.Z1 <= second.Z2;
    }

    private static IList<Instruction> FilterInitializationProcedure(IList<Instruction> instructions)
    {
        var index = instructions.IndexOf(instructions.First(x =>
            !(x.Cuboid.X1.IsBetween(-50, 50) && x.Cuboid.X2.IsBetween(-50, 50))));

        return instructions
            .Take(index)
            .ToList();
    }

    private IList<Instruction> GetInstructions()
    {
        return Utilities.GetInput(GetType())
            .Split(Environment.NewLine)
            .Select(x => (Type: x[..3].Trim(),
                Coords: Regex.Matches(x, @"-?\d+").Select(m => int.Parse(m.Value)).ToArray()))
            .Select(x => new Instruction(x.Type, new Cuboid(
                Math.Min(x.Coords[0], x.Coords[1]),
                Math.Max(x.Coords[0], x.Coords[1]),
                Math.Min(x.Coords[2], x.Coords[3]),
                Math.Max(x.Coords[2], x.Coords[3]),
                Math.Min(x.Coords[4], x.Coords[5]),
                Math.Max(x.Coords[4], x.Coords[5]))))
            .ToList();
    }

    private record Cuboid(long X1, long X2, long Y1, long Y2, long Z1, long Z2);
    private record Instruction(string Type, Cuboid Cuboid);
}