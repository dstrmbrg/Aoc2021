using System;
using System.Collections.Generic;
using System.Linq;

namespace Aoc2021.Puzzles;

internal class Day12 : Puzzle
{
    public override object PartOne()
    {
        var start = GetStartNode();

        return GetPathCount(new HashSet<Node>(), start, true);
    }

    public override object PartTwo()
    {
        var start = GetStartNode();

        return GetPathCount(new HashSet<Node>(), start, false);
    }

    private static int GetPathCount(ISet<Node> path, Node node, bool limitToExactlyOnce)
    {
        if (!limitToExactlyOnce && node.IsLimited && path.Contains(node))
            limitToExactlyOnce = true;

        path.Add(node);

        if (node.Name == "end")
            return 1;
        
        var validNodes = node.ConnectedNodes
            .Where(x => x.Name != "start")
            .Where(x => !x.IsLimited || !limitToExactlyOnce || !path.Contains(x))
            .ToList();
        
        return validNodes.None() ? 0 : validNodes.Sum(x => GetPathCount(path.ToHashSet(), x, limitToExactlyOnce));
    }

    private Node GetStartNode()
    {
        var lines = Utilities.GetInput(GetType())
            .Split(Environment.NewLine);

        var nodes = lines
            .SelectMany(x => x.Split("-"))
            .Distinct()
            .Select(x => new Node(x))
            .ToList();

        foreach (var line in lines)
        {
            var split = line.Split("-");
            var (first, second) = (nodes.Single(n => n.Name == split[0]), nodes.Single(n => n.Name == split[1]));

            first.ConnectedNodes.Add(second);
            second.ConnectedNodes.Add(first);
        }

        return nodes.Single(x => x.Name == "start");
    }

    private class Node
    {
        public Node(string name)
        {
            Name = name;
            ConnectedNodes = new List<Node>();
        }

        public string Name { get; set; }
        public IList<Node> ConnectedNodes { get; }

        public bool IsLimited => Name == Name.ToLower();
    }
}