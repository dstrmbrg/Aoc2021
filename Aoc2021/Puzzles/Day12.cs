using System;
using System.Collections.Generic;
using System.Linq;

namespace Aoc2021.Puzzles;

internal class Day12 : Puzzle
{
    public override object PartOne() => GetPathCount(new HashSet<Node>(), GetStartNode(), false);

    public override object PartTwo() => GetPathCount(new HashSet<Node>(), GetStartNode(), true);

    private static int GetPathCount(HashSet<Node> visitedLimitedNodes, Node node, bool allowVisitOneLimitedNodeTwice)
    {
        if (node.IsEnd) return 1;

        if (allowVisitOneLimitedNodeTwice && node.LimitedVisits && visitedLimitedNodes.Contains(node))
            allowVisitOneLimitedNodeTwice = false;

        if (node.LimitedVisits)
            visitedLimitedNodes = new HashSet<Node>(visitedLimitedNodes, visitedLimitedNodes.Comparer) { node };

        return node.ConnectedNodes
            .Where(x => !x.IsStart && (!x.LimitedVisits || allowVisitOneLimitedNodeTwice || !visitedLimitedNodes.Contains(x)))
            .Sum(x => GetPathCount(visitedLimitedNodes, x, allowVisitOneLimitedNodeTwice));
    }

    private Node GetStartNode()
    {
        var paths = Utilities.GetInput(GetType())
            .Split(Environment.NewLine)
            .Select(x => x.Split("-").ToArray())
            .Select(x => (Start: x[0], Destination: x[1]));

        var nodesDict = new Dictionary<string, Node>();

        foreach (var (start, destination) in paths)
        {
            nodesDict.TryAdd(start, new Node(start));
            nodesDict.TryAdd(destination, new Node(destination));

            var (startNode, destinationNode) = (nodesDict[start], nodesDict[destination]);

            startNode.ConnectedNodes.Add(destinationNode);
            destinationNode.ConnectedNodes.Add(startNode);
        }

        return nodesDict["start"];
    }

    private class Node
    {
        public readonly bool IsStart;
        public readonly bool IsEnd;
        public readonly bool LimitedVisits;

        public Node(string name)
        {
            ConnectedNodes = new List<Node>();
            IsStart = name == "start";
            IsEnd = name == "end";
            LimitedVisits = name == name.ToLower();
        }

        public IList<Node> ConnectedNodes { get; }
    }
}