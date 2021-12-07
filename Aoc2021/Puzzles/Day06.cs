using System.Linq;

namespace Aoc2021.Puzzles
{
    internal class Day06 : Puzzle
    {
        public override object PartOne() => CalculateFishPopulation(80);

        public override object PartTwo() => CalculateFishPopulation(256);

        private long CalculateFishPopulation(int days)
        {
            var state = GetInitialState();
            state = Enumerable.Range(0, days).Aggregate(state, (current, _) => CalculateNextState(current));
            return state.Sum();
        }

        private static long[] CalculateNextState(long[] state)
        {
            var nextState = state
                .Skip(1)
                .Concat(state.Take(1))
                .ToArray();
            nextState[6] += state[0];
            return nextState;
        }

        private long[] GetInitialState()
        {
            var lookup = Utilities.GetInput(GetType())
                .Split(",")
                .Select(int.Parse)
                .ToLookup(x => x);

            return Enumerable.Range(0, 9)
                .Select(i => (long)lookup[i].Count())
                .ToArray();
        }
    }
}