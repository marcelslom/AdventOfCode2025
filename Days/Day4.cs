using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Days
{
    public class Day4 : ISolution
    {
        public string Part1(string[] input)
        {
            var result = 0;
            var board = input.Select(x => x.ToCharArray()).ToArray();
            for (var y = 0; y < input.Length; y++)
            {
                for (var x = 0; x < input[y].Length; x++)
                {
                    if (input[y][x] != '@')
                    {
                        continue;
                    }

                    var neighbours = FindNumberOfNeighbours(board, y, x);
                    if (neighbours < 4)
                    {
                        result++;
                    }
                }
            }

            return result.ToString();
        }

        public static int FindNumberOfNeighbours(char[][] board, int y, int x)
        {
            var neighbours = 0;
            for (var dy = -1; dy <= 1; dy++)
            {
                if (y + dy < 0 || y + dy >= board.Length)
                {
                    continue;
                }

                for(var dx = -1; dx <= 1; dx++)
                {
                    if (x + dx < 0 || x + dx >= board[y].Length)
                    {
                        continue;
                    }

                    if (dy == 0 && dx == 0)
                    {
                        continue;
                    }

                    if (board[y + dy][x + dx] == '@')
                    {
                        neighbours++;
                    }
                }
            }

            return neighbours;
        }

        public string Part2(string[] input)
        {
            var result = 0;
            var board = input.Select(x => x.ToCharArray()).ToArray();

            while (true)
            {
                var rollsToRemove = RollsToRemove(board);
                if (rollsToRemove.Count == 0)
                {
                    break;
                }

                RemoveRolls(board, rollsToRemove);
                result += rollsToRemove.Count;
            }

            return result.ToString();
        }

        public static List<(int y, int x)> RollsToRemove(char[][] board)
        {
            var result = new List<(int y, int x)>();

            for (var y = 0; y < board.Length; y++)
            {
                for (var x = 0; x < board[y].Length; x++)
                {
                    if (board[y][x] != '@')
                    {
                        continue;
                    }

                    var neighbours = FindNumberOfNeighbours(board, y, x);
                    if (neighbours < 4)
                    {
                        result.Add((y, x));
                    }
                }
            }

            return result;
        }

        public static void RemoveRolls(char[][] board, List<(int y, int x)> rollsToRemove)
        {
            foreach(var roll in rollsToRemove)
            {
                board[roll.y][roll.x] = '.';
            }
        }
    }
}
