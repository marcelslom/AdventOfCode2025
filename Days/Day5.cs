namespace Days
{
    public class Day5 : ISolution
    {
        public string Part1(string[] input)
        {
            var ranges = ParseRanges(input);
            var ids = ParseIds(input);
            var result = ids.Count(id => ranges.Any(range => id >= range.min && id <= range.max));
            return result.ToString();
        }

        public static List<(long min, long max)> ParseRanges(string[] input)
        {
            var result = new List<(long min, long max)>();
            foreach (var range in input)
            {
                if (string.IsNullOrWhiteSpace(range))
                {
                    break;
                }

                var splitted = range.Split('-');
                var parsed = (long.Parse(splitted[0]), long.Parse(splitted[1]));
                result.Add(parsed);
            }

            return result;
        }

        public static List<long> ParseIds(string[] input)
        {
            var result = new List<long>();
            var found = false;
            foreach (var id in input)
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    found = true;
                    continue;
                }

                if (!found)
                {
                    continue;
                }

                var parsed = long.Parse(id);
                result.Add(parsed);
            }

            return result;
        }

        public static bool Consolidate(List<(long min, long max)> ranges, out List<(long min, long max)> consolidated)
        {
            List<(long min, long max)> result = [];
            result.Add(ranges[0]);
            var modified = false;
            for (var i = 1; i < ranges.Count; i++)
            {
                var range = ranges[i];
                if (result.Any(r => r.min <= range.min && r.max >= range.max))
                {
                    modified = true;
                    continue;
                }

                var rangeToReplaceIndex = result.TakeWhile(r => !(r.min >= range.min && r.max <= range.max)).Count();
                if (rangeToReplaceIndex < result.Count)
                {
                    result[rangeToReplaceIndex] = range;
                    modified = true;
                    continue;
                }

                var rangeToModifyDownIndex = result.TakeWhile(r => !(range.min < r.min && range.max >= r.min - 1 && range.max <= r.max)).Count();
                if (rangeToModifyDownIndex < result.Count)
                {
                    var rangeToModifyDown = result[rangeToModifyDownIndex];
                    var newRange = (range.min, rangeToModifyDown.max);
                    result[rangeToModifyDownIndex] = newRange;
                    modified = true;
                    continue;
                }

                var rangeToModifyUpIndex = result.TakeWhile(r => !(range.max > r.max && range.min >= r.min && range.min <= r.max + 1)).Count();
                if (rangeToModifyUpIndex < result.Count)
                {
                    var rangeToModifyUp = result[rangeToModifyUpIndex];
                    var newRange = (rangeToModifyUp.min, range.max);
                    result[rangeToModifyUpIndex] = newRange;
                    modified = true;
                    continue;
                }

                result.Add(range);
            }

            consolidated = result;
            return modified;
        }

        public string Part2(string[] input)
        {
            var ranges = ParseRanges(input);
            while (true)
            {
                var modified = Consolidate(ranges, out ranges);
                if (!modified)
                {
                    break;
                }
            }

            var result = 0L;
            foreach (var range in ranges)
            {
                result += range.max - range.min + 1;
            }

            return result.ToString();
        }
    }
}
