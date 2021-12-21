using System;
using System.Collections.Generic;
using System.Linq;

namespace Aoc2021.Puzzles;

internal class Day21 : DisabledPuzzle
{
    public override object PartOne() => SimulateDeterministicDice();

    public override object PartTwo() => SimulateQuantumDice();

    private int SimulateDeterministicDice()
    {
        var dice = new DeterministicDice();
        var positions = ReadInput();
        var scores = new [] { 0, 0 };

        var turn = 0;

        while (scores.All(x => x < 1000))
        {
            positions[turn] = GetNewPosition(positions[turn], dice.RollMultiple(3));
            scores[turn] += positions[turn];

            turn = (turn + 1) % 2;
        }

        return dice.Rolls * scores.Min();
    }

    private long SimulateQuantumDice()
    {
        var positions = ReadInput();
        var stack = new Stack<Board>();

        var playerOneWins = 0L;
        var playerTwoWins = 0L;

        var combinations = new (int Roll, int Count)[]
        {
            (3, 1),
            (4, 3),
            (5, 6),
            (6, 7),
            (7, 6),
            (8, 3),
            (9, 1)
        };

        foreach (var (roll, count) in combinations)
            stack.Push(new Board(positions[0], positions[1], 0, 0, count, 0, roll));

        while (stack.Count != 0)
        {
            var (playerOnePosition, playerTwoPosition, playerOneScore, playerTwoScore, combinationCount, turn, roll) = stack.Pop();
            
            if (turn == 0)
            {
                playerOnePosition = GetNewPosition(playerOnePosition, roll);
                playerOneScore += playerOnePosition;
            }
            else
            {
                playerTwoPosition = GetNewPosition(playerTwoPosition, roll);
                playerTwoScore += playerTwoPosition;
            }
            
            if (playerOneScore >= 21)
            {
                playerOneWins += combinationCount;
                continue;
            }

            if (playerTwoScore >= 21)
            {
                playerTwoWins += combinationCount;
                continue;
            }

            turn = (turn + 1) % 2;

            foreach (var (nextRoll, count) in combinations)
                stack.Push(new Board(playerOnePosition, playerTwoPosition, playerOneScore, playerTwoScore, combinationCount * count, turn, nextRoll));
        }

        return Math.Max(playerOneWins, playerTwoWins);
    }

    private static int GetNewPosition(int position, int steps)
    {
        var newPosition = position + steps;

        if (newPosition <= 10)
            return newPosition;

        if (newPosition % 10 == 0)
            return 10;

        return newPosition % 10;
    }

    private int[] ReadInput()
    {
        return Utilities.GetInput(GetType())
            .Split(Environment.NewLine)
            .Select(x => (int)char.GetNumericValue(x[^1]))
            .ToArray();
    }

    private class DeterministicDice
    {
        private int _current;
        public int Rolls;

        private int Roll()
        {
            Rolls++;
            _current = _current == 100 ? 1 : _current + 1;
            return _current;
        }

        public int RollMultiple(int count) => Enumerable.Range(0, count).Sum(_ => Roll());
    }

    private record Board(int PlayerOnePosition, int PlayerTwoPosition, long PlayerOneScore,
        long PlayerTwoScore, long CombinationCount, int Turn, int Roll);
}