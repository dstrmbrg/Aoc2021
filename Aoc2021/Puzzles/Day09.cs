using System;
using System.Collections.Generic;
using System.Linq;

namespace Aoc2021.Puzzles;
internal class Day09 : Puzzle
{
    private static int _xMax;
    private static int _yMax;

    public override object PartOne()
    {
        var map = GetLocations().ToDictionary(p => (p.X, p.Y));
        
        return map.Where(p => GetAdjacentLocations(map, p.Key.X, p.Key.Y).All(a => a.Height > p.Value.Height))
            .Sum(p => p.Value.Height + 1);
    }

    public override object PartTwo()
    {
        var map = GetLocations().ToDictionary(p => (p.X, p.Y));
        
        return map.Where(p => GetAdjacentLocations(map, p.Key.X, p.Key.Y).All(a => a.Height > p.Value.Height))
            .Select(x => GetBasinSize(map, x.Key.X, x.Key.Y))
            .OrderByDescending(x => x)
            .Take(3)
            .Aggregate(1, (current, basin) => current * basin);
    }

    private static IEnumerable<Location> GetAdjacentLocations(IDictionary<(int, int), Location> map, int x, int y)
    {
        return new (int X, int Y)[] { (x - 1, y), (x + 1, y), (x, y - 1), (x, y + 1) }
            .Where(a => a.X.IsBetween(0, _xMax) && a.Y.IsBetween(0, _yMax))
            .Select(a => map[(a.X, a.Y)]);
    }

    private static int GetBasinSize(IDictionary<(int, int), Location> map, int x, int y) => GetBasin(map, x, y).Distinct().Count() + 1;

    private static IEnumerable<Location> GetBasin(IDictionary<(int, int), Location> map, int x, int y)
    {
        var higherAdjacentLocations = GetAdjacentLocations(map, x, y)
            .Where(a => a.Height != 9 && a.Height > map[(x, y)].Height)
            .ToArray();

        return higherAdjacentLocations.Concat(higherAdjacentLocations.SelectMany(a => GetBasin(map, a.X, a.Y)));
    }

    private IEnumerable<Location> GetLocations()
    {
        var rows = Utilities.GetInput(GetType())
            .Split(Environment.NewLine);

        _yMax = rows.Length - 1;
        _xMax = rows[0].Length - 1;

        for (var y = 0; y < rows.Length; y++)
        for (var x = 0; x < rows[y].Length; x++)
            yield return new Location(x, y, (int)char.GetNumericValue(rows[y][x]));
    }

    private record Location(int X, int Y, int Height);
}