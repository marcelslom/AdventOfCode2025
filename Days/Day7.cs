using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Days
{
    public class Day7 : ISolution
    {
        public string Part1(string[] input)
        {
            var start = input[0].IndexOf('S');
            var space = new bool[input[0].Length];
            space[start] = true;
            var splits = 0;
            foreach (var row in input.Skip(2))
            {
                var newSpace = new bool[space.Length];
                for (var i = 0; i < space.Length; i++)
                {
                    if (!space[i])
                    {
                        continue;
                    }

                    if(row[i] == '^')
                    {
                        newSpace[i - 1] = true;
                        newSpace[i + 1] = true;
                        splits++;
                    }
                    else
                    {
                        newSpace[i] = true;
                    }
                }

                space = newSpace;
            }

            return splits.ToString();
        }

        public string Part2(string[] input)
        {
            var start = input[0].IndexOf('S');
            var possibilities = new long[input[0].Length];
            possibilities[start] = 1;
            foreach (var row in input.Skip(2))
            {
                var newPossibilities = new long[possibilities.Length];
                for (var i = 0; i < row.Length; i++)
                {
                    if (row[i] == '^')
                    {
                        newPossibilities[i - 1] += possibilities[i];
                        newPossibilities[i + 1] += possibilities[i];
                    }
                    else
                    {
                        newPossibilities[i] += possibilities[i];
                    }
                }

                possibilities = newPossibilities;
            }

            return possibilities.Sum().ToString();
        }
    }
}
