using System;
using System.Collections.Generic;
using System.Text;

namespace Days
{
    public class Day6 : ISolution
    {
        public string Part1(string[] input)
        {
            var problems = ParseInputPart1(input);
            var result = problems.Select(x => x.Calculate()).Sum();
            return result.ToString();
        }

        private static List<Problem> ParseInputPart1(string[] input)
        {
            var problems = input[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => new Problem(long.Parse(x))).ToList();
            foreach (var line in input.Skip(1))
            {
                var splitted = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (long.TryParse(splitted[0], out _)) {
                    for (var i = 0; i < splitted.Length; i++)
                    {
                        problems[i].Operands.Add(long.Parse(splitted[i]));
                    }
                }
                else
                {
                    for (var i = 0; i < splitted.Length; i++)
                    {
                        problems[i].Operation = splitted[i];
                    }
                }

            }

            return problems;
        }

        private static List<Problem> ParseInputPart2(string[] input)
        {
            List<Problem> problems = [];
            var problem = new Problem();
            var separator = false;
            for (var i = input[0].Length - 1; i >= 0; i--)
            {
                if (separator)
                {
                    separator = false;
                    continue;
                }

                var operand = string.Empty;
                for (var j = 0; j < input.Length - 1; j++)
                {
                    operand += input[j][i];
                }

                problem.Operands.Add(int.Parse(operand));

                if (input[input.Length - 1][i] != ' ')
                {
                    problem.Operation = input[input.Length - 1][i].ToString();
                    problems.Add(problem);
                    problem = new();
                    separator = true;
                }

            }

            return problems;
        }

        public string Part2(string[] input)
        {
            var problems = ParseInputPart2(input);
            var result = problems.Select(x => x.Calculate()).Sum();
            return result.ToString();
        }

        class Problem
        {
            public List<long> Operands { get; } = [];
            public string Operation { get; set; }

            public Problem(long firstOperand)
            {
                Operands.Add(firstOperand); 
            }

            public Problem()
            {
            }

            public long Calculate()
            {
                if (Operation == "+")
                {
                    return Operands.Sum();
                }
                else if (Operation == "*")
                {
                    return Operands.Aggregate(1L, (x, y) => x * y);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}
