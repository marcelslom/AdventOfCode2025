using System;
using System.Collections.Generic;
using System.Text;

namespace Days
{
    public class Day3 : ISolution
    {
        public string Part1(string[] input)
        {
            var result = 0;
            foreach (var bank in input)
            {
                var tens = FindTheGreatestJoltage(bank, 0, bank.Length - 2);
                var ones = FindTheGreatestJoltage(bank, tens.index + 1, bank.Length - 1);
                var bankJoltage = 10 * tens.value + ones.value;
                result += bankJoltage;
            }

            return result.ToString();
        }

        public static (int index, byte value) FindTheGreatestJoltage(string bank, int startIndex, int endIndex)
        {
            (int index, byte value) result = (-1, 0);
            for (var i = startIndex; i < endIndex + 1; i++)
            {
                var joltage = (byte)(bank[i] - '0');
                if (joltage > result.value)
                {
                    result.value = joltage;
                    result.index = i;
                }

                if (joltage == 9)
                {
                    break;
                }
            }

            return result;
        }

        public string Part2(string[] input)
        {
            var result = 0L;
            foreach (var bank in input)
            {
                var joltage = 0L;
                var startIndex = 0;
                for (var i = 0; i < 12; i++)
                {
                    var digit = FindTheGreatestJoltage(bank, startIndex, bank.Length - 12 + i);
                    joltage += (long)Math.Pow(10, 11 - i) * digit.value;
                    startIndex = digit.index + 1;
                }

                result += joltage;
            }

            return result.ToString();
        }
    }
}
