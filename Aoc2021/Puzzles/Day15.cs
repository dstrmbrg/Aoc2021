using System;
using System.Collections.Generic;
using System.Linq;

namespace Aoc2021.Puzzles;

internal class Day15 : Puzzle
{
    public override object PartOne()
    {
        var locations = GetLocations().ToArray();
        var (start, goal) = (locations.First(), locations.Last());
        var map = locations.ToDictionary(l => (l.X, l.Y));

        return CalculateLowestRisk(map, start, goal);
    }

    public override object PartTwo()
    {
        return "-1";
    }

    private static long CalculateLowestRisk(IDictionary<(int, int), Location> map, Location start, Location goal)
    {
        var queue = new Queue<(Location Location, long Risk)>();
        var visited = new Dictionary<Location, long>();

        queue.Enqueue((start, -start.Risk));
        var lowest = long.MaxValue;

        while (queue.Count != 0)
        {
            var (location, risk) = queue.Dequeue();

            risk += location.Risk;
            
            if (location == goal)
                if (risk < lowest)
                {
                    lowest = risk;
                    continue;
                }

            var adjacentLocations = GetAdjacentLocations(map, location)
                .Where(a => !visited.TryGetValue(a, out var previousRisk) || risk + a.Risk < previousRisk)
                .OrderBy(a => a.X)
                .ThenBy(a => a.Y);

            foreach (var adjacentLocation in adjacentLocations)
            {
                var nextRisk = risk + adjacentLocation.Risk;

                if (visited.ContainsKey(adjacentLocation))
                    visited[adjacentLocation] = nextRisk;
                else
                    visited.Add(adjacentLocation, nextRisk);

                queue.Enqueue((adjacentLocation, risk));
            }
        }
        
        return lowest;
    }
    
    private static IEnumerable<Location> GetAdjacentLocations(IDictionary<(int, int), Location> map, Location location)
    {
        return new (int X, int Y)[] { (location.X + 1, location.Y), (location.X, location.Y + 1), (location.X - 1, location.Y), (location.X, location.Y - 1) }
            .Where(a => map.ContainsKey((a.X, a.Y)))
            .Select(a => map[(a.X, a.Y)]);
    }

    private IEnumerable<Location> GetLocations()
    {
        var rows = Utilities.GetInput(GetType())
            .Split(Environment.NewLine);
        
        for (var y = 0; y < rows.Length; y++)
        for (var x = 0; x < rows[y].Length; x++)
            yield return new Location(x, y, (int)char.GetNumericValue(rows[y][x]));
    }

    private record Location(int X, int Y, int Risk);
}