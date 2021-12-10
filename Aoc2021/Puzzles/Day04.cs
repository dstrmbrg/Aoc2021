using System;
using System.Collections.Generic;
using System.Linq;

namespace Aoc2021.Puzzles;

internal class Day04 : Puzzle
{
    public override object PartOne() => CalculateWinningScore(IncreasingDrawnNumbersSequence);

    public override object PartTwo() => CalculateWinningScore(DecreasingDrawnNumbersSequence);

    private int CalculateWinningScore(Func<IList<int>, IEnumerable<IList<int>>> drawnNumbersSequenceFunc)
    {
        var (drawnNumbers, boards) = ReadInput();

        return drawnNumbersSequenceFunc(drawnNumbers)
            .Select(numbers => (DrawnNumbers: numbers,
                Board: boards.SingleOrDefault(x => IsFirstBingoRound(x, numbers))))
            .Where(x => x.Board != null)
            .Select(x => CalculateScore(x.DrawnNumbers, x.Board))
            .FirstOrDefault();
    }

    private static int CalculateScore(IList<int> drawnNumbers, int[][] board) => drawnNumbers.Last() * board.SelectMany(x => x).Except(drawnNumbers).Sum();

    private static IEnumerable<IList<int>> IncreasingDrawnNumbersSequence(IList<int> drawnNumbers) => Enumerable.Range(0, drawnNumbers.Count).Select(i => drawnNumbers.Take(i).ToList());

    private static IEnumerable<IList<int>> DecreasingDrawnNumbersSequence(IList<int> drawnNumbers) => Enumerable.Range(0, drawnNumbers.Count).Select(i => drawnNumbers.SkipLast(i).ToList());

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