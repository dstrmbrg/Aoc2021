using System;
using System.Collections.Generic;
using System.Linq;
using WinstonPuckett.PipeExtensions;

namespace Aoc2021.Puzzles;

internal class Day18 : DisabledPuzzle
{
    public override object PartOne() => GetPairs()
        .Pipe(Simulate)
        .Pipe(x => x.GetMagnitude());

    public override object PartTwo()
    {
        var pairs = GetPairs();
        var count = pairs.Length;
        var max = 0;

        for (var i = 0; i < count; i++)
        {
            for (var j = 0; j < count; j++)
            {
                if (i == j) continue;

                pairs = GetPairs();

                var array = new[]
                {
                    pairs[i],
                    pairs[j]
                };

                var result = Simulate(array);
                var magnitude = result.GetMagnitude();

                if (magnitude > max)
                    max = magnitude;
            }
        }

        return max;
    }

    private static Pair Simulate(Pair[] pairs)
    {
        return pairs.Skip(1).Aggregate(pairs[0], AddPairs);
    }

    private static Pair AddPairs(Pair left, Pair right)
    {
        var pair = new Pair(null)
        {
            Left = left,
            Right = right
        };
        pair.Left.Parent = pair;
        pair.Right.Parent = pair;
        Reduce(pair);

        return pair;
    }

    private static void Reduce(Pair pair)
    {
        while (true)
        {
            var recalculate = HandleExplosions(pair);
            recalculate |= HandleSplit(pair);

            if (!recalculate)
                break;
        }
    }

    private static bool HandleExplosions(Pair pair)
    {
        var explodingPairs = pair.GetAllChildren()
            .Where(p => p.Depth() >= 4 && p.Left != null && p.Right != null)
            .ToList();

        if (explodingPairs.Count == 0) return false;

        foreach (var explodingPair in explodingPairs)
        {
            explodingPair.ExplodeLeft();
            explodingPair.ExplodeRight();

            explodingPair.Left = null;
            explodingPair.Right = null;
            explodingPair.Value = 0;
        }

        return true;
    }

    private static bool HandleSplit(Pair pair)
    {
        var splittingPair = pair
            .GetAllChildren()
            .FirstOrDefault(p => p.Value >= 10);

        if (splittingPair == null) return false;

        var value = splittingPair.Value;
        var leftValue = value / 2;
        var rightValue = (value + 1) / 2;
        
        splittingPair.Value = null;
        splittingPair.Left = new Pair(splittingPair) { Value = leftValue };
        splittingPair.Right = new Pair(splittingPair) { Value = rightValue };

        return true;
    }

    private Pair[] GetPairs()
    {
        return Utilities.GetInput(GetType())
            .Split(Environment.NewLine)
            .Select(x => ParsePair(x))
            .ToArray();
    }

    private static Pair ParsePair(string pairString, Pair parent = null)
    {
        var pair = new Pair(parent);

        if (int.TryParse(pairString, out var value))
        {
            pair.Value = value;
            return pair;
        }

        var content = pairString[1..^1];
        var openings = 0;
        var closings = 0;

        for (var i = 0; i < content.Length; i++)
        {
            switch (content[i])
            {
                case ',' when openings == closings:
                {
                    pair.Left = ParsePair(content[..i], pair);
                    pair.Right = ParsePair(content[(i + 1)..], pair);
                    
                    return pair;
                }
                case '[':
                    openings += 1;
                    break;
                case ']':
                    closings += 1;
                    break;
            }
        }

        return pair;
    }

    private class Pair
    {
        public int? Value;
        
        public Pair Parent;
        public Pair Left;
        public Pair Right;

        public Pair(Pair parent)
        {
            Parent = parent;
        }

        public int Depth()
        {
            var depth = 0;
            var current = this;
            while (current.Parent != null)
            {
                depth += 1;
                current = current.Parent;
            }

            return depth;
        }

        public IList<Pair> GetAllChildren()
        {
            if (Value.HasValue)
                return new List<Pair> { this };

            var result = new List<Pair>() { this };

            var leftChildren = Left.GetAllChildren();
            var rightChildren = Right.GetAllChildren();
            result.AddRange(leftChildren);
            result.AddRange(rightChildren);

            return result;
        }

        public void ExplodeLeft()
        {
            var root = GetRoot();
            var children = root.GetAllChildren();
            var currentIndex = children.IndexOf(this);

            var firstLeft = children
                .Where((x, i) => i < currentIndex && x.Value.HasValue && x.Parent != this)
                .LastOrDefault();

            if (firstLeft == null)
                return;

            firstLeft.Value += Left.Value;
        }

        public void ExplodeRight()
        {
            var root = GetRoot();
            var children = root.GetAllChildren();
            var currentIndex = children.IndexOf(this);

            var firstRight = children
                .Where((x, i) => i > currentIndex && x.Value.HasValue && x.Parent != this)
                .FirstOrDefault();

            if (firstRight == null)
                return;

            firstRight.Value += Right.Value;
        }

        public int GetMagnitude()
        {
            if (Left == null || Right == null)
                return Value ?? 0;

            return Left.GetMagnitude() * 3 + Right.GetMagnitude() * 2;
        }

        private Pair GetRoot()
        {
            return Parent == null ? this : Parent.GetRoot();
        }
    }
}