using System;
using System.Collections.Generic;
using System.Linq;

namespace Aoc2021.Puzzles;

internal class Day15 : DisabledPuzzle
{
    public override object PartOne() => CalculateLowestRisk(false);

    public override object PartTwo() => CalculateLowestRisk(true);

    private int CalculateLowestRisk(bool megaMap)
    {
        var locations = GetLocations().ToArray();
        var (start, goal) = (locations.First(), locations.Last());
        var map = locations.ToDictionary(l => (l.X, l.Y));

        var width = goal.X + 1;
        var height = goal.Y + 1;
        
        if (megaMap)
        {
            var newRisk = goal.Risk + 8 < 10 ? goal.Risk + 8 : (goal.Risk + 8) % 10 + 1;
            goal = new Location(goal.X + 4 * width, goal.Y + 4 * height, newRisk);
        }

        var queue = new PriorityQueue<(Location Location, int Risk), int>();
        var visited = new Dictionary<Location, int>();

        queue.Enqueue((start, -start.Risk), 1);
        var lowest = int.MaxValue;

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

            var adjacentLocations = GetAdjacentLocations(map, location, megaMap, width, height)
                .Where(a => !visited.TryGetValue(a, out var previousRisk) || risk + a.Risk < previousRisk)
                .OrderBy(a => goal.X - a.X + goal.Y - a.Y)
                .ThenBy(a => a.Risk);

            foreach (var adjacentLocation in adjacentLocations)
            {
                var nextRisk = risk + adjacentLocation.Risk;

                if (visited.ContainsKey(adjacentLocation))
                    visited[adjacentLocation] = nextRisk;
                else
                    visited.Add(adjacentLocation, nextRisk);

                queue.Enqueue((adjacentLocation, risk), risk);
            }
        }
        
        return lowest;
    }

    private static IEnumerable<Location> GetAdjacentLocations(IDictionary<(int, int), Location> map, Location location, bool megaMap, int width, int height)
    {
        var multiplier = megaMap ? 5 : 1;

        return new (int X, int Y)[] { (location.X + 1, location.Y), (location.X, location.Y + 1), (location.X - 1, location.Y), (location.X, location.Y - 1) }
            .Where(a => a.X.IsBetween(0, width * multiplier - 1) && a.Y.IsBetween(0, height * multiplier - 1))
            .Select(a => GetLocation(map, (a.X, a.Y), megaMap, width, height));
    }
    
    private static Location GetLocation(IDictionary<(int X, int Y), Location> dict, (int X, int Y) key, bool megaMap, int width, int height)
    {
        if (!megaMap)
            return dict[key];

        if (dict.TryGetValue(key, out var location))
            return location;
        
        var offset = (int)(Math.Floor((decimal)key.X / width) + Math.Floor((decimal)key.Y / height));
        var x = key.X % width;
        var y = key.Y % height;
        
        var originalLocation = dict[(x, y)];
        var newRisk = originalLocation.Risk + offset < 10 
            ? originalLocation.Risk + offset : 
            (originalLocation.Risk + offset) % 10 + 1;

        var newLocation = new Location(key.X, key.Y, newRisk);

        return newLocation;
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