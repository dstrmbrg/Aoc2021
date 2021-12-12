using System;
using System.Collections.Generic;
using System.Linq;

namespace Aoc2021.Puzzles;

internal class Day12 : Puzzle
{
    public override object PartOne()
    {
        var start = GetStartNode();

        return GetPaths(new List<Node>(), start, true).Count;
    }

    public override object PartTwo()
    {
        var start = GetStartNode();

        return GetPaths(new List<Node>(), start, false).Count;
    }

    private static IList<IList<Node>> GetPaths(IList<Node> path, Node node, bool limitToExactlyOnce)
    {
        path.Add(node);

        if (node.Name == "end")
            return new List<IList<Node>> { path };

        var validNodes = node.ConnectedNodes
            .Where(x => x.Name != "start")
            .Where(x =>
                !x.IsLimited || !path.Contains(x) ||
                !limitToExactlyOnce && !path.GroupBy(y => y).Any(z => z.Key.IsLimited && z.Count() == 2))
            .ToList();
        
        return validNodes.None() ? new List<IList<Node>>() : validNodes.Aggregate(new List<IList<Node>>(), (current, validNode) => current.Concat(GetPaths(path.ToList(), validNode, limitToExactlyOnce)).ToList());
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