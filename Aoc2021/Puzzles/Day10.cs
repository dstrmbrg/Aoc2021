using System;
using System.Collections.Generic;
using System.Linq;

namespace Aoc2021.Puzzles
{
    internal class Day10 : Puzzle
    {
        public override object PartOne()
        {
            var chunks = GetChunks();

            var corruptedChunks = chunks
                .Select(x => ValidateChunk(x, new Stack<char>()))
                .Where(x => x.ChunkEnum == ChunkEnum.Corrupted);

            return corruptedChunks.Sum(x => ToPoints(x.FirstInvalidCharacter));
        }

        public override object PartTwo()
        {
            var chunks = GetChunks();

            var incompleteChunks = chunks
                .Select(x => ValidateChunk(x, new Stack<char>()))
                .Where(x => x.ChunkEnum == ChunkEnum.Incomplete);

            var scores = incompleteChunks.Select(x => GetScore(x.RemainingCharacters.ToArray())).ToList();

            var middleScore = scores.OrderBy(x => x)
                .Skip(scores.Count / 2)
                .First();

            return middleScore;
        }

        private (ChunkEnum ChunkEnum, char? FirstInvalidCharacter, Stack<char> RemainingCharacters) ValidateChunk(char[] chunk, Stack<char> buffer)
        {
            if (chunk.Length == 0)
            {
                return buffer.Count == 0 ? (ChunkEnum.Valid, null, buffer) : (ChunkEnum.Incomplete, null, buffer);
            }

            if (Instructions.Select(x => x.Start).Contains(chunk[0]))
            {
                buffer.Push(chunk[0]);
                return ValidateChunk(chunk.Skip(1).ToArray(), buffer);
            }

            var instruction = Instructions.Single(x => x.End == chunk[0]);

            if (buffer.Peek() != instruction.Start) return (ChunkEnum.Corrupted, chunk[0], buffer);
            
            buffer.Pop();
            return ValidateChunk(chunk.Skip(1).ToArray(), buffer);
        }

        private IList<char[]> GetChunks()
        {
            return Utilities.GetInput(GetType())
                .Split(Environment.NewLine)
                .Select(x => x.ToCharArray())
                .ToList();
        }

        private int GetScore(char[] characters)
        {
            var score = 0;

            foreach (var character in characters)
            {
                score *= 5;
                score += ToPoints2(GetEndInstuction(character));
            }

            return score;
        }

        private static int ToPoints(char? c) =>
            c switch
            {
                ')' => 3,
                ']' => 57,
                '}' => 1197,
                '>' => 25137,
                _ => 0
            };

        private static int ToPoints2(char? c) =>
            c switch
            {
                ')' => 1,
                ']' => 2,
                '}' => 3,
                '>' => 4,
                _ => 0
            };

        private char GetEndInstuction(char startInstruction)
        {
            return Instructions.Single(x => x.Start == startInstruction).End;
        }

        private Instruction[] Instructions => new[]
        {
            new Instruction('(', ')'), new Instruction('[', ']'), new Instruction('{', '}'), new Instruction('<', '>')
        };

        private record Instruction(char Start, char End);

        private enum ChunkEnum
        {
            Valid,
            Incomplete,
            Corrupted
        }
    }
}