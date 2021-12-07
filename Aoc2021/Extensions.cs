using System;
using System.Collections.Generic;

namespace Aoc2021
{
    internal static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var element in source)
            {
                action(element);
            }
        }

        public static LinkedListNode<T> GetPrevious<T>(this LinkedListNode<T> node)
        {
            return node.Previous ?? node.List.Last;
        }

        public static LinkedListNode<T> GetNext<T>(this LinkedListNode<T> node)
        {
            return node.Next ?? node.List.First;
        }

        public static LinkedListNode<T> GetPrevious<T>(this LinkedListNode<T> node, int steps)
        {
            for (var i = 0; i < steps; i++)
            {
                node = node.GetPrevious();
            }

            return node;
        }

        public static LinkedListNode<T> GetNext<T>(this LinkedListNode<T> node, int steps)
        {
            for (var i = 0; i < steps; i++)
            {
                node = node.GetNext();
            }

            return node;
        }

        public static bool IsBetween<T>(this T item, T start, T end)
        {
            if (Comparer<T>.Default.Compare(start, end) > 0)
            {
                (start, end) = (end, start);
            }

            return Comparer<T>.Default.Compare(item, start) >= 0
                   && Comparer<T>.Default.Compare(item, end) <= 0;
        }

    }
}
