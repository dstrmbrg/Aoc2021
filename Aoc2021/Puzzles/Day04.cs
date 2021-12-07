using System;
using System.Collections.Generic;
using System.Linq;

namespace Aoc2021.Puzzles
{
    internal class Day04 : Puzzle
    {
        public override object PartOne()
        {
            var (drawnNumbers, boards) = ReadInput();
            var (round, board) = GetWinners(boards, drawnNumbers).First();
            return CalculateScore(drawnNumbers, round, board);
        }

        public override object PartTwo()
        {
            var (drawnNumbers, boards) = ReadInput();
            var (round, board) = GetWinners(boards, drawnNumbers).Last();
            return CalculateScore(drawnNumbers, round, board);
        }

        private static IList<(int Round, int[][] Board)> GetWinners(IList<int[][]> boards, IList<int> drawnNumbers)
        {
            return Enumerable.Range(1, drawnNumbers.Count)
                .Select(drawnNumbersCount => (drawnNumbersCount, board: boards.FirstOrDefault(x => IsFirstBingoRound(x, drawnNumbers.Take(drawnNumbersCount).ToList()))))
                .Where(x => x.board != null)
                .OrderByDescending(x => x.drawnNumbersCount)
                .ToList();
        }

        private static int CalculateScore(IList<int> drawnNumbers, int round, int[][] board)
        {
            var finalDrawnNumbers = drawnNumbers.Take(round).ToList();
            var sum = board.SelectMany(x => x).Except(finalDrawnNumbers).Sum();
            return sum * drawnNumbers[round - 1];
        }

        private (IList<int> drawnNumbers, IList<int[][]> boards) ReadInput()
        {
            var input = Utilities.GetInput(GetType());
            var split = input.Split(Environment.NewLine + Environment.NewLine);
            var drawnNumbers = split[0].Split(",").Select(int.Parse).ToList();
            var boardStrings = split.Skip(1).ToList();
            var boards = boardStrings.Select(ParseBoard).ToList();
            return (drawnNumbers, boards);
        }

        private static int[][] ParseBoard(string boardString)
        {
            var rows = boardString.Split(Environment.NewLine);

            return Enumerable.Range(0, 5)
                .Select(x => Enumerable.Range(0, 5)
                    .Select(y => int.Parse(rows[x].Split(" ", StringSplitOptions.RemoveEmptyEntries)[y]))
                    .ToArray()).ToArray();
        }

        private static bool IsFirstBingoRound(int[][] board, IList<int> drawnNumbers)
        {
            return IsBingo(board, drawnNumbers) && !IsBingo(board, drawnNumbers.SkipLast(1).ToList());
        }

        private static bool IsBingo(int[][] board, IList<int> drawnNumbers)
        {
            return Enumerable.Range(0, board.Length)
                .Any(x => GetRow(board, x).All(drawnNumbers.Contains) || GetColumn(board, x).All(drawnNumbers.Contains));
        }

        private static int[] GetRow(int[][] board, int row) => board[row];

        private static int[] GetColumn(int[][] board, int col) => board.Select(x => x[col]).ToArray();
    }
}