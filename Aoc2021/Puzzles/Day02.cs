using System;
using System.Collections.Generic;
using System.Linq;

namespace Aoc2021.Puzzles
{
    internal class Day02 : Puzzle
    {
        public override object PartOne()
        {
            var instructions = GetInstructions();
            var horizontal = instructions.Where(x => x.type == "forward").Sum(x => x.value);
            var depth = instructions.Where(x => x.type == "down").Sum(x => x.value) -
                        instructions.Where(x => x.type == "up").Sum(x => x.value);

            return depth * horizontal;
        }

        public override object PartTwo()
        {
            var instructions = GetInstructions();
            var horizontal = instructions.Where(x => x.type == "forward").Sum(x => x.value);
            var depth = instructions.Select((instruction, index) => (instruction, index))
                .Where(x => x.instruction.type == "forward").Sum(x =>
                    x.instruction.value * instructions
                        .Where((y, index) => index < x.index && (y.type == "up" || y.type == "down"))
                        .Sum(z => z.value * (z.type == "down" ? 1 : -1)));

            return depth * horizontal;
        }

        private List<(string type, int value)> GetInstructions()
        {
            var input = Utilities.GetInput(GetType());
            return input.Split(Environment.NewLine)
                .Select(x => x.Split(" "))
                .Select(x => (Type: x[0], Value: Convert.ToInt32(x[1]))).ToList();
        }
    }
}