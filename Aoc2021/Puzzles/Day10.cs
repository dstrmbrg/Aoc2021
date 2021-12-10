using System;
using System.Collections.Generic;
using System.Linq;

namespace Aoc2021.Puzzles
{
    internal class Day10 : Puzzle
    {
        public override object PartOne()
        {
            return GetChunks()
                .Select(x => ValidateChunk(x, new Stack<char>()))
                .Where(x => x.ChunkStatusEnum == ChunkStatusEnum.Corrupted)
                .Sum(x => GetChunkOperator(x.FirstInvalidCharacter).FirstScore);
        }

        public override object PartTwo()
        {
            var scores = GetChunks()
                .Select(x => ValidateChunk(x, new Stack<char>()))
                .Where(x => x.ChunkStatusEnum == ChunkStatusEnum.Incomplete)
                .Select(x => CalculateScore(x.RemainingCharacters.ToArray()))
                .OrderBy(x => x)
                .ToArray();

            return scores[(scores.Length - 1) / 2];
        }

        private static (ChunkStatusEnum ChunkStatusEnum, char? FirstInvalidCharacter, Stack<char> RemainingCharacters) ValidateChunk(Queue<char> chunk, Stack<char> openings)
        {
            if (chunk.Count == 0)
            {
                return openings.Count == 0 ? (ChunkStatusEnum.Valid, null, openings) : (ChunkStatusEnum.Incomplete, null, openings);
            }

            var currentCharacter = chunk.Dequeue();

            if (IsClosingCharacter(currentCharacter))
                return openings.Pop() == GetChunkOperator(currentCharacter).Opening ? ValidateChunk(chunk, openings) : (ChunkStatusEnum.Corrupted, currentCharacter, openings);

            openings.Push(currentCharacter);
            return ValidateChunk(chunk, openings);
        }

        private static long CalculateScore(char[] characters) => characters.Aggregate<char, long>(0, (sum, c) => sum * 5 + GetChunkOperator(c).SecondScore);
        private static ChunkOperator GetChunkOperator(char? op) => ChunkOperators.Single(x => x.Opening == op || x.Closing == op);
        private static bool IsClosingCharacter(char op) => ChunkOperators.Any(x => x.Closing == op);

        private static ChunkOperator[] ChunkOperators => new[]
        {
            new ChunkOperator('(', ')', 3, 1), 
            new ChunkOperator('[', ']', 57, 2), 
            new ChunkOperator('{', '}', 1197, 3), 
            new ChunkOperator('<', '>', 25137, 4)
        };

        private IList<Queue<char>> GetChunks()
        {
            return Utilities.GetInput(GetType())
                .Split(Environment.NewLine)
                .Select(x => new Queue<char>(x.ToCharArray()))
                .ToList();
        }

        private record ChunkOperator(char Opening, char Closing, int FirstScore, int SecondScore);

        private enum ChunkStatusEnum
        {
            Valid,
            Incomplete,
            Corrupted
        }
    }
}