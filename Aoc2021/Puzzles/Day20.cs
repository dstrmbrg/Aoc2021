using System;
using System.Collections.Generic;
using System.Linq;

namespace Aoc2021.Puzzles;

internal class Day20 : DisabledPuzzle
{
    public override object PartOne()
    {
        var (algorithm, map) = ReadInput();
        Enhance(map, algorithm, 2);

        return map.Count(x => x.Value == 1);
    }

    public override object PartTwo()
    {
        var (algorithm, map) = ReadInput();
        Enhance(map, algorithm, 50);

        return map.Count(x => x.Value == 1);
    }
    
    private static void Enhance(IDictionary<(int X, int Y), int> map, string algorithm, int steps, int currentStep = 0)
    {
        if (steps == currentStep)
            return;

        var oldMap = map.ToDictionary(x => x.Key, x => x.Value);

        var xMin = oldMap.Min(x => x.Key.X);
        var xMax = oldMap.Max(x => x.Key.X);
        var yMin = oldMap.Min(x => x.Key.Y);
        var yMax = oldMap.Max(x => x.Key.Y);

        for (var x = xMin - 1; x <= xMax + 1; x++)
        for (var y = yMin - 1; y <= yMax + 1; y++)
            map[(x, y)] = GetPixel(oldMap, algorithm, x, y, currentStep);

        Enhance(map, algorithm, steps, ++currentStep);
    }

    private static int GetPixel(IDictionary<(int X, int Y), int> map, string algorithm, int x, int y, int currentStep)
    {
        var binaryString = string.Empty;

        for (var j = y - 1; j <= y + 1; j++)
        for (var i = x - 1; i <= x + 1; i++)
        {
            map.TryAdd((i, j), GetDefaultValue(algorithm, currentStep));
            binaryString += map[(i, j)];
        }

        return (int)char.GetNumericValue(algorithm[Convert.ToInt32(binaryString, 2)]);
    }

    private static int GetDefaultValue(string algorithm, int currentStep)
    {
        if (currentStep == 0)
            return 0;

        var first = (int)char.GetNumericValue(algorithm[0]);
        var last = (int)char.GetNumericValue(algorithm[^1]);
        var flickering = first == 1 && last == 0;

        if (!flickering)
            return first;

        return currentStep % 2 == 0 ? last : first;
    }
    
    private (string Algorithm, IDictionary<(int X, int Y), int> Map) ReadInput()
    {
        var split = Utilities.GetInput(GetType())
            .Split($"{Environment.NewLine}{Environment.NewLine}");
        var algorithm = split[0];
        algorithm = algorithm.Replace('#', '1');
        algorithm = algorithm.Replace('.', '0');
        var rows = split[1].Split(Environment.NewLine);
        var map = new Dictionary<(int X, int Y), int>();
        var y = 0;

        foreach (var row in rows)
        {
            for (var x = 0; x < row.Length; x++) 
                map.Add((x, y), row[x] == '#' ? 1 : 0);

            y += 1;
        }

        return (algorithm, map);
    }
}