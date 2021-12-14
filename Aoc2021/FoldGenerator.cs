using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Aoc2021
{
    internal static class FoldGenerator
    {
        internal static void Generate()
        {
            var dots = ReadInput();
            var folds = string.Empty;

            for (var i = 0; i < 12; i++)
            {
                var axis = i % 2 == 0 ? 'y' : 'x';

                folds = UnFold(dots, axis) + folds;
            }

            var dotsString = string.Join(Environment.NewLine, dots.Select(d => $"{d.X},{d.Y}"));

            var output = dotsString + Environment.NewLine + Environment.NewLine + folds;

            File.WriteAllText("C:\\temp\\folds.txt", output);
        }

        private static string UnFold(IList<Dot> dots, char axis)
        {
            var rand = new Random();
            var reflectAt = (axis == 'y' ? dots.Max(d => d.Y) : dots.Max(d => d.X)) + rand.Next(1, 20);
            var index = 0;

            foreach (var dot in dots.ToList())
            {
                if (rand.Next(1, 3) == 2)
                {
                    if (axis == 'y')
                        dots[index] = new Dot(dot.X, Reflect(dot.Y, reflectAt));
                    else
                        dots[index] = new Dot(Reflect(dot.X, reflectAt), dot.Y);

                    if (rand.Next(1, 11) == 10)
                        dots.Add(dot);
                }

                index++;
            }

            return $"fold along {axis}={reflectAt}{Environment.NewLine}";
        }

        private static int Reflect(int position, int reflectAt) => 2 * reflectAt - position;

        private static IList<Dot> ReadInput()
        {
            var input = File.ReadAllText(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "\\fold_input.txt");

            var lines = input.Split(Environment.NewLine);

            var dots = new List<Dot>();

            for (var y = 0; y < lines.Length; y++)
            for (var x = 0; x < lines[y].Length; x++)
                if (lines[y][x] == '#')
                    dots.Add(new Dot(x, y));

            return dots;
        }

        private record Dot(int X, int Y);
    }
}
