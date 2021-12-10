using System;
using System.IO;

namespace Aoc2021;

internal static class Utilities
{
    public static string GetInput(Type dayType, bool test = false)
    {
        var folder = test ? "\\Inputs\\Test\\" : "\\Inputs\\";

        var filePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + $"{folder}{dayType.Name}.txt";
        return File.ReadAllText(filePath);
    }
}