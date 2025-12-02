using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Text;

namespace Days
{
    public class Day2 : ISolution
    {
        public bool UseRealInput => true;

        public string Part1(string[] input)
        {
            var ranges = input[0].Split(',');
            var sum = ranges.Select(x => x.Split('-')).Select(x => GetInvalidIdsPart1(x).Sum()).Sum();
            return sum.ToString();
        }

        public IEnumerable<long> GetInvalidIdsPart1(string[] range)
        {
            var min = long.Parse(range[0]);
            var nextIdHalfLength = range[0].Length / 2;
            nextIdHalfLength = nextIdHalfLength > 1 ? nextIdHalfLength : 1;
            var nextIdHalf = long.Parse(range[0][0..(nextIdHalfLength)]);
            var max = long.Parse(range[1]);
            while (true)
            {
                var next = long.Parse(nextIdHalf.ToString() + nextIdHalf.ToString());
                if (next > max)
                {
                    yield break;
                }

                if (next >= min && next <= max)
                {
                    yield return next;
                }

                nextIdHalf++;
            }
        }

        public IEnumerable<long> GetInvalidIdsPart2(string[] range)
        {
            var min = long.Parse(range[0]);
            var max = long.Parse(range[1]);
            var maxSegmentLength = range[1].Length / 2 + 1;
            for (var segmentLength = 1; segmentLength <= maxSegmentLength; segmentLength++)
            {
                var minNumberOfSegments = Math.Max(2, range[0].Length / segmentLength);
                var maxNumberOfSegments = range[1].Length / segmentLength + 1;

                for (var numberOfSegments = minNumberOfSegments; numberOfSegments <= maxNumberOfSegments; numberOfSegments++)
                {
                    for (var segment = Math.Pow(10, segmentLength - 1); segment < Math.Pow(10, segmentLength); segment++)
                    {
                        var next = long.Parse(string.Concat(Enumerable.Repeat(segment.ToString(), numberOfSegments)));
                        if (next > max)
                        {
                            break;
                        }

                        if (next >= min && next <= max)
                        {
                            yield return next;
                        }                   
                    }
                }
            }
        }

        public string Part2(string[] input)
        {
            var ranges = input[0].Split(',');
            var sum = ranges.Select(x => x.Split('-')).Select(x => GetInvalidIdsPart2(x).Distinct().Sum()).Sum();
            return sum.ToString();
        }
    }
}
