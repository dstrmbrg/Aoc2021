﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Aoc2021.Puzzles
{
    internal class Day09 : Puzzle
    {
        private static int _xMax;
        private static int _yMax;

        public override object PartOne()
        {
            var map = GetLocations().ToDictionary(p => (p.X, p.Y), p => p.Height);
            var lowest = map.Where(p => GetAdjacentLocations(map, p.Key.X, p.Key.Y).All(a => a.Height > p.Value));
            return lowest.Sum(p => p.Value + 1);
        }

        public override object PartTwo()
        {
            var map = GetLocations().ToDictionary(p => (p.X, p.Y), p => p.Height);
            var lowest = map.Where(p => GetAdjacentLocations(map, p.Key.X, p.Key.Y).All(a => a.Height > p.Value));
            var basinSizes = lowest.Select(x => GetBasinSize(map, x.Key.X, x.Key.Y));

            var threeLargestBasins = basinSizes
                .OrderByDescending(x => x)
                .Take(3);

            return threeLargestBasins.Aggregate(1, (current, basin) => current * basin);
        }

        private static IEnumerable<Location> GetAdjacentLocations(IDictionary<(int, int), int> map, int x, int y)
        {
            var adjacent = new List<(int X, int Y)>
            {
                (x - 1, y),
                (x + 1, y),
                (x, y - 1),
                (x, y + 1)
            };

            return adjacent
                .Where(a => a.X.IsBetween(0, _xMax) && a.Y.IsBetween(0, _yMax))
                .Select(a => new Location(a.X, a.Y, map[(a.X, a.Y)]));
        }

        private static int GetBasinSize(IDictionary<(int, int), int> map, int x, int y) => GetBasin(map, x, y).Distinct().Count() + 1;

        private static IList<Location> GetBasin(IDictionary<(int, int), int> map, int x, int y)
        {
            var higherAdjacentLocations = GetAdjacentLocations(map, x, y)
                .Where(a => a.Height != 9 && a.Height > map[(x, y)])
                .ToList();

            return higherAdjacentLocations.Concat(higherAdjacentLocations.SelectMany(a => GetBasin(map, a.X, a.Y))).ToList();
        }

        private IEnumerable<Location> GetLocations()
        {
            var rows = Utilities.GetInput(GetType())
                .Split(Environment.NewLine);

            _yMax = rows.Length - 1;
            _xMax = rows[0].Length - 1;

            for (var y = 0; y < rows.Length; y++)
            {
                for (var x = 0; x < rows[y].Length; x++)
                {
                    yield return new Location(x, y, (int)char.GetNumericValue(rows[y][x]));
                }
            }
        }

        private record Location(int X, int Y, int Height)
        {
            public readonly int X = X;
            public readonly int Y = Y;
            public readonly int Height = Height;
        }
    }
}