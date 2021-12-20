using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Aoc2021.Puzzles;

internal class Day19 : DisabledPuzzle
{
    public override object PartOne()
    {
        var scanners = ReadScanners();

        return Solve(scanners).Count;
    }

    public override object PartTwo()
    {
        var scanners = ReadScanners();

        return Solve(scanners).MaxDistance;
    }

    private (int Count, int MaxDistance) Solve(IList<Scanner> scanners)
    {
        var omniScanner = scanners[0];
        var done = new List<Scanner> { scanners[0] };
        var scannerPositions = new List<Beacon> { new(0, 0, 0) };

        while (true)
        {
            foreach (var scanner in scanners.Except(done))
            {
                var newBeacons = GetBeaconsFromScanner(omniScanner, scanner, out var scannerPosition);

                if (newBeacons.Count == 0) continue;

                foreach (var newBeacon in newBeacons)
                    if (!omniScanner.Beacons.Contains(newBeacon))
                        omniScanner.Beacons.Add(newBeacon);

                scannerPositions.Add(scannerPosition);
                done.Add(scanner);

                Console.WriteLine($"Total: {scanners.Count}. Done: {done.Count}");
            }
            
            if (scanners.Count == done.Count) break;
        }

        return (omniScanner.Beacons.Count, GetMaxDistance(scannerPositions));
    }

    private static int GetMaxDistance(IList<Beacon> scannerPositions)
    {
        var max = 0;

        foreach (var sc in scannerPositions)
        foreach (var sc2 in scannerPositions)
        {
            var distance = CalculateManhattanDistance(sc, sc2);

            if (distance > max)
                max = distance;
        }

        return max;
    }

    private static int CalculateManhattanDistance(Beacon first, Beacon second)
    {
        return Math.Abs(first.X - second.X) + Math.Abs(first.Y - second.Y) + Math.Abs(first.Z - second.Z);
    }
    
    private static IList<Beacon> GetBeaconsFromScanner(Scanner first, Scanner second, out Beacon scannerPosition)
    {
        var doneFirstScanner = new List<Beacon>();

        foreach (var firstPairOne in first.Beacons)
        {
            doneFirstScanner.Add(firstPairOne);
            foreach (var secondPairOne in first.Beacons.Except(doneFirstScanner))
            foreach (var firstPairTwo in second.Beacons.AsParallel())
            foreach (var secondPairTwo in second.Beacons)
                if (IsMatchingPairs(firstPairOne, secondPairOne, firstPairTwo, secondPairTwo, out var map))
                {
                    var func = GetProjection(firstPairOne, secondPairOne, firstPairTwo, secondPairTwo, map);
                    var projections = second.Beacons.Select(x => func(x)).ToList();
                    var count = first.Beacons.Count(x => projections.Contains(x));

                    if (count >= 3)
                    {
                        scannerPosition = func(new Beacon(0, 0, 0));
                        return projections;
                    }
                }
        }

        scannerPosition = new Beacon(0, 0, 0);
        return new List<Beacon>();
    }

    private static Func<Beacon, Beacon> GetProjection(Beacon firstPairOne, Beacon secondPairOne,
        Beacon firstPairTwo, Beacon secondPairTwo, IDictionary<Axis, Axis> map)
    {
        return beacon => new Beacon(GetProjection(firstPairOne, secondPairOne, firstPairTwo, secondPairTwo, map, Axis.X).Invoke(beacon),
            GetProjection(firstPairOne, secondPairOne, firstPairTwo, secondPairTwo, map, Axis.Y).Invoke(beacon),
            GetProjection(firstPairOne, secondPairOne, firstPairTwo, secondPairTwo, map, Axis.Z).Invoke(beacon));
    }

    private static Func<Beacon, int> GetProjection(Beacon firstPairOne, Beacon secondPairOne,
        Beacon firstPairTwo, Beacon secondPairTwo, IDictionary<Axis, Axis> map, Axis axis)
    {
        var fromAxis = map[axis];

        var f1 = 0;
        var f2 = 0;

        switch (fromAxis)
        {
            case Axis.X:
                f1 = firstPairTwo.X;
                f2 = secondPairTwo.X;
                break;
            case Axis.Y:
                f1 = firstPairTwo.Y;
                f2 = secondPairTwo.Y;
                break;
            case Axis.Z:
                f1 = firstPairTwo.Z;
                f2 = secondPairTwo.Z;
                break;
        }

        var t1 = 0;
        var t2 = 0;

        switch (axis)
        {
            case Axis.X:
                t1 = firstPairOne.X;
                t2 = secondPairOne.X;
                break;
            case Axis.Y:
                t1 = firstPairOne.Y;
                t2 = secondPairOne.Y;
                break;
            case Axis.Z:
                t1 = firstPairOne.Z;
                t2 = secondPairOne.Z;
                break;
        }

        if (f2 - f1 == 0)
            return _ => 0;

        var k = (t2 - t1) / (f2 - f1);
        var m = t1 - k * f1;

        return fromAxis switch
        {
            Axis.X => beacon => k * beacon.X + m,
            Axis.Y => beacon => k * beacon.Y + m,
            Axis.Z => beacon => k * beacon.Z + m,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    

    private static bool IsMatchingPairs(Beacon firstPairOne, Beacon secondPairOne, Beacon firstPairTwo, Beacon secondPairTwo, out IDictionary<Axis, Axis> map)
    {
        map = new Dictionary<Axis, Axis>();

        if (Math.Abs(CalculateDistance(firstPairOne, secondPairOne)
                     - CalculateDistance(firstPairTwo, secondPairTwo)) > 0.01d)
            return false;
        
        var firstList = new List<(Axis Axis, int Distance)>
        {
            (Axis.X, Math.Abs(firstPairOne.X - secondPairOne.X)),
            (Axis.Y, Math.Abs(firstPairOne.Y - secondPairOne.Y)),
            (Axis.Z, Math.Abs(firstPairOne.Z - secondPairOne.Z))
        }.OrderBy(x => x.Distance).ToList();

        var secondList = new List<(Axis Axis, int Distance)>
        {
            (Axis.X, Math.Abs(firstPairTwo.X - secondPairTwo.X)),
            (Axis.Y, Math.Abs(firstPairTwo.Y - secondPairTwo.Y)),
            (Axis.Z, Math.Abs(firstPairTwo.Z - secondPairTwo.Z))
        }.OrderBy(x => x.Distance).ToList();
        
        if (!firstList.Select(x => x.Distance).SequenceEqual(secondList.Select(x => x.Distance))) 
            return false;

        map = firstList
            .Select((x, i) => (From: x.Axis, To: secondList[i].Axis))
            .ToDictionary(x => x.From, x => x.To);

        return true;
    }

    private static double CalculateDistance(Beacon first, Beacon second) => Math.Sqrt(Math.Pow(first.X - second.X, 2) +
                                                                                      Math.Pow(first.Y - second.Y, 2) +
                                                                                      Math.Pow(first.Z - second.Z, 2));

    private IList<Scanner> ReadScanners()
    {
        return Utilities.GetInput(GetType())
            .Split($"{Environment.NewLine}{Environment.NewLine}")
            .Select(sc => sc
                .Split(Environment.NewLine)
                .Skip(1)
                .Select(x => Regex.Matches(x, @"-?\d+").Select(m => int.Parse(m.Value)).ToArray())
                .Select(b => new Beacon(b[0], b[1], b[2]))
                .ToHashSet())
            .Select(beacons => new Scanner(beacons))
            .ToList();
    }
    
    private class Scanner
    {
        public readonly ISet<Beacon> Beacons;

        public Scanner(ISet<Beacon> beacons)
        {
            Beacons = beacons;
        }
    }

    private record Beacon(int X, int Y, int Z);

    private enum Axis
    {
        X,
        Y,
        Z
    }
}