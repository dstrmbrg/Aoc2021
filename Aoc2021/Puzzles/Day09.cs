using System;
using System.Collections.Generic;
using System.Linq;

namespace Aoc2021.Puzzles
{
    internal class Day09 : Puzzle
    {
        public override object PartOne()
        {
            var map = GetPositions().ToDictionary(p => (p.X, p.Y), p => p.Height);
            var lowest = map.Where(p => GetAdjacentHeights(map, p.Key.X, p.Key.Y).All(a => a > p.Value));
            return lowest.Sum(p => p.Value + 1);
        }

        public override object PartTwo()
        {
            var map = GetPositions().ToDictionary(p => (p.X, p.Y), p => p.Height);

            var lowest = map.Where(p => GetAdjacentHeights(map, p.Key.X, p.Key.Y).All(a => a > p.Value));

            var basins = lowest
                .AsParallel()
                .Select(x => GetBasinSize(map, x.Key.X, x.Key.Y));

            var largest = basins.OrderByDescending(x => x)
                .Take(3)
                .ToArray();

            return largest.Aggregate(1, (current, basin) => current * basin);
        }

        private IList<(int X, int Y)> GetAdjacentPositions(IDictionary<(int X, int Y), int> map, int x, int y)
        {
            var xMax = map.Max(p => p.Key.X);
            var yMax = map.Max(p => p.Key.Y);

            var adjacent = new List<(int X, int Y)>
            {
                (x - 1, y),
                (x + 1, y),
                (x, y - 1),
                (x, y + 1)
            };

            return adjacent
                .Where(a => a.X.IsBetween(0, xMax) && a.Y.IsBetween(0, yMax))
                .ToList();
        }

        private int[] GetAdjacentHeights(IDictionary<(int X, int Y), int> map, int x, int y)
        {
            return GetAdjacentPositions(map, x, y)
                .Select(a => map[(a.X, a.Y)])
                .ToArray();
        }

        private IList<(int X, int Y)> GetAdjacentHigherPositions(IDictionary<(int X, int Y), int> map, int x, int y)
        {
            return GetAdjacentPositions(map, x, y)
                .Where(a => map[(a.X, a.Y)] != 9 && map[(a.X, a.Y)] > map[(x, y)])
                .ToList();
        }

        private int GetBasinSize(IDictionary<(int X, int Y), int> map, int x, int y)
        {
            var basin = GetBasin(map, x, y);

            return basin.Distinct().Count() + 1;
        }

        private IList<(int X, int Y)> GetBasin(IDictionary<(int X, int Y), int> map, int x, int y)
        {
            var adjacent = GetAdjacentHigherPositions(map, x, y);
            
            return adjacent.Concat(adjacent.SelectMany(a => GetBasin(map, a.X, a.Y))).ToList();
        }


        private IEnumerable<(int X, int Y, int Height)> GetPositions()
        {
            var rows = Utilities.GetInput(GetType())
                .Split(Environment.NewLine);

            for (var y = 0; y < rows.Length; y++)
            {
                for (var x = 0; x < rows[y].Length; x++)
                {
                    yield return (x, y, (int)char.GetNumericValue(rows[y][x]));
                }

            }
        }
    }
}