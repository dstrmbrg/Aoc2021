namespace Aoc2021.Puzzles;

public abstract class Puzzle : DisabledPuzzle
{

}

public abstract class DisabledPuzzle
{
    public abstract object PartOne();
    public abstract object PartTwo();
}