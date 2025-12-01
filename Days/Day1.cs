using System;
using System.Collections.Generic;
using System.Text;

namespace Days
{
    public class Day1 : ISolution
    {
        public bool UseRealInput => true;

        public string Part1(string[] input)
        {
            var position = 50;
            var result = 0;
            foreach (var item in input) 
            {
                var direction = item[0];
                var clicks = int.Parse(item.Substring(1));
                clicks %= 100;
                if (direction == 'R')
                {
                    position += clicks;
                    position %= 100;
                }
                else if (direction == 'L')
                {
                    position -= clicks;
                    position %= 100;
                    if (position < 0)
                    {
                        position += 100;
                    }
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(direction));
                }

                if (position == 0)
                {
                    result++; 
                }
            }

            return result.ToString();
        }

        public string Part2(string[] input)
        {
            var position = 50;
            var result = 0;
            foreach (var item in input)
            {
                var direction = item[0];
                var clicks = int.Parse(item.Substring(1));
                var fullRotations = clicks / 100;
                if (fullRotations > 0)
                {
                    result += fullRotations;
                }
                clicks %= 100;
                var oldPosition = position;
                if (direction == 'R')
                {
                    position += clicks;
                    if (position > 100)
                    {
                        position %= 100;
                        if (oldPosition != 0)
                        {
                            result++;
                        }
                    }
                }
                else if (direction == 'L')
                {
                    position -= clicks;
                    position %= 100;
                    if (position < 0)
                    {
                        position += 100;
                        if (oldPosition != 0)
                        {
                            result++;
                        }
                    }
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(direction));
                }

                if (position == 0 || position == 100)
                {
                    position = 0;
                    result++;
                }
            }

            return result.ToString();
        }

    }
}
