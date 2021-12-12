using System;
using System.Collections.Generic;
using System.Linq;

namespace Aoc2021.Puzzles;

internal class Day12 : Puzzle
{
    public override object PartOne() => GetPathCount(Array.Empty<Node>(), GetStartNode(), true);

    public override object PartTwo() => GetPathCount(Array.Empty<Node>(), GetStartNode(), false);

    private static int GetPathCount(Node[] visitedLimitedNodes, Node node, bool limitToExactlyOnce)
    {
        if (node.IsEnd) return 1;

        if (!limitToExactlyOnce && node.IsLimited && visitedLimitedNodes.Contains(node))
            limitToExactlyOnce = true;

        var validNodes = node.ConnectedNodes
            .Where(x => !x.IsStart)
            .Where(x => !x.IsLimited || !limitToExactlyOnce || !visitedLimitedNodes.Contains(x))
            .ToArray();

        if (validNodes.Length == 0) return 0;

        visitedLimitedNodes = CopyAndAppendArray(visitedLimitedNodes, node);
        return validNodes.Sum(x => GetPathCount(visitedLimitedNodes, x, limitToExactlyOnce));
    }

    private static Node[] CopyAndAppendArray(Node[] source, Node node)
    {
        var length = node.IsLimited ? source.Length + 1 : source.Length;
        var destination = new Node[length];
        Array.Copy(source, destination, source.Length);

        if (node.IsLimited)
            destination[^1] = node;
        return destination;
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
            if (!nodesDict.ContainsKey(start))
                nodesDict.Add(start, new Node(start));

            if (!nodesDict.ContainsKey(destination))
                nodesDict.Add(destination, new Node(destination));

            var (startNode, destinationNode) = (nodesDict[start], nodesDict[destination]);

            startNode.ConnectedNodes.Add(destinationNode);
            destinationNode.ConnectedNodes.Add(startNode);
        }

        return nodesDict["start"];
    }

    private class Node
    {
        public readonly bool IsEnd;
        public readonly bool IsLimited;
        public readonly bool IsStart;

        public Node(string name)
        {
            ConnectedNodes = new List<Node>();
            IsStart = name == "start";
            IsEnd = name == "end";
            IsLimited = name == name.ToLower();
        }

        public IList<Node> ConnectedNodes { get; }
    }
}