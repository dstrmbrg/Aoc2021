using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Aoc2021.Puzzles
{
    internal class Day05Old : DisabledPuzzle
    {
        public override object PartOne()
        {
            var lines = ReadInput();
            var overlaps = GetNonDiagonalOverlaps(lines);
            return overlaps.SelectMany(x => x).Count(x => x >= 2);
        }
        public override object PartTwo()
        {
            var lines = ReadInput();
            var overlaps = GetCompleteOverlaps(lines);
            return overlaps.SelectMany(x => x).Count(x => x >= 2);
        }

        private static int[][] GetCompleteOverlaps(IList<((int x, int y) start, (int x, int y) end)> lines)
        {
            var maxPos = Math.Max(lines.Max(line => Math.Max(line.start.x, line.end.x)), lines.Max(line => Math.Max(line.start.y, line.end.y)));
            return Enumerable.Range(0, maxPos + 1)
                .AsParallel()
                .Select(x => Enumerable.Range(0, maxPos + 1).Select(y => GetCompleteOverlap(lines, x, y)).ToArray()).ToArray();
        }

        private static int[][] GetNonDiagonalOverlaps(IList<((int x, int y) start, (int x, int y) end)> lines)
        {
            var maxPos = Math.Max(lines.Max(line => Math.Max(line.start.x, line.end.x)), lines.Max(line => Math.Max(line.start.y, line.end.y)));
            return Enumerable.Range(0, maxPos + 1)
                .AsParallel()
                .Select(x => Enumerable.Range(0, maxPos + 1).Select(y => GetNonDiagonalOverlap(lines, x, y)).ToArray()).ToArray();
        }
        
        private static int GetNonDiagonalOverlap(IList<((int x, int y) start, (int x, int y) end)> lines, int xPos, int yPos)
        {
            return lines
                .Where(line => line.start.x == line.end.x || line.start.y == line.end.y)
                .Count(line => xPos.IsBetween(line.start.x, line.end.x) && yPos.IsBetween(line.start.y, line.end.y));
        }

        private static int GetCompleteOverlap(IList<((int x, int y) start, (int x, int y) end)> lines, int xPos, int yPos)
        {
            var nonDiagonalOverlap = GetNonDiagonalOverlap(lines, xPos, yPos);

            return nonDiagonalOverlap + lines
                .Where(line => xPos.IsBetween(line.start.x, line.end.x) && yPos.IsBetween(line.start.y, line.end.y))
                .Count(line => Math.Abs(xPos - line.start.x) == Math.Abs(yPos - line.start.y)
                                     && Math.Abs(xPos - line.end.x) == Math.Abs(yPos - line.end.y));
        }

        private IList<((int x, int y) start, (int x, int y) end)> ReadInput()
        {
            return Utilities.GetInput(typeof(Day05))
                .Split(Environment.NewLine)
                .Select(row => 
                    Regex.Matches(row, @"-?\d+")
                    .Select(x => int.Parse(x.Value))
                    .ToArray())
                .Select(x => ((x[0], x[1]), (x[2], x[3])))
                .ToList();
        }
    }
}
