using System;
using System.Collections.Generic;
using System.Linq;

namespace Aoc2021.Puzzles;

internal class Day11 : Puzzle
{
    public override object PartOne() => SimulateOctopuses(100).FlashingCount;

    public override object PartTwo() => SimulateOctopuses(-1).SynchronizedAtStep;

    private (int FlashingCount, int SynchronizedAtStep) SimulateOctopuses(int steps)
    {
        var map = GetOctopuses().ToDictionary(p => (p.X, p.Y));

        return SimulateOctopusStep(map, steps);
    }

    private static (int FlashingCount, int SynchronizedAtStep) SimulateOctopusStep(IDictionary<(int X, int Y), Octopus> map, int steps, int currentStep = 0)
    {
        if (currentStep == steps) return (0, 0);
        if (steps == -1 && map.All(x => x.Value.EnergyLevel == 0)) return (0, currentStep);

        map
            .Select(x => x.Value)
            .ForEach(IncreaseEnergyLevel);

        var flashingCount = SimulateFlashing(map, Array.Empty<Octopus>());
        var nextStep = SimulateOctopusStep(map, steps, ++currentStep);
        return (flashingCount + nextStep.FlashingCount, nextStep.SynchronizedAtStep);
    }

    private static int SimulateFlashing(IDictionary<(int X, int Y), Octopus> map, Octopus[] flashingOctopuses)
    {
        var newFleshingOctopuses = map
            .Where(x => x.Value.EnergyLevel == 0)
            .Where(x => !flashingOctopuses.Contains(x.Value))
            .Select(x => x.Value)
            .ToArray();

        if (newFleshingOctopuses.Length == 0) return 0;
        
        newFleshingOctopuses
            .SelectMany(o => GetAdjacentOctopuses(map, o))
            .Where(x => x.EnergyLevel != 0)
            .ForEach(IncreaseEnergyLevel);

        flashingOctopuses = flashingOctopuses
            .Concat(newFleshingOctopuses)
            .Distinct()
            .ToArray();

        return newFleshingOctopuses.Length + SimulateFlashing(map, flashingOctopuses);
    }

    private static IEnumerable<Octopus> GetAdjacentOctopuses(IDictionary<(int, int), Octopus> map, Octopus octopus)
    {
        for (var xOffset = -1; xOffset <= 1; xOffset++)
        for (var yOffset = -1; yOffset <= 1; yOffset++)
        {
            if ((octopus.X + xOffset).IsBetween(0, 9) && (octopus.Y + yOffset).IsBetween(0, 9))
                yield return map[(octopus.X + xOffset, octopus.Y + yOffset)];
        }
    }

    private static void IncreaseEnergyLevel(Octopus octopus) => octopus.EnergyLevel = (octopus.EnergyLevel + 1) % 10;

    private IEnumerable<Octopus> GetOctopuses()
    {
        var rows = Utilities.GetInput(GetType())
            .Split(Environment.NewLine);
        
        for (var y = 0; y < rows.Length; y++)
        for (var x = 0; x < rows[y].Length; x++)
            yield return new Octopus(x, y, (int)char.GetNumericValue(rows[y][x]));
    }

    private class Octopus
    {
        public readonly int X;
        public readonly int Y;
        public int EnergyLevel;

        public Octopus(int x, int y, int energyLevel)
        {
            X = x;
            Y = y;
            EnergyLevel = energyLevel;
        }
    }
}