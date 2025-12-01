using Days;

namespace Runner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var daysAssembly = typeof(ISolution).Assembly;
            var day = daysAssembly
                .GetTypes()
                .Where(x => x.Name.StartsWith("Day"))
                .Select(x => new { Type = x, Number = int.Parse(x.Name.Replace("Day", string.Empty)) })
                .OrderByDescending(x => x.Number)
                .First();

            var instance = (ISolution)Activator.CreateInstance(day.Type);

            Console.WriteLine($"Running day {day.Number}...");
            var filename = instance.UseRealInput ? "input.txt" : "example.txt";
            try
            {
                var input = File.ReadLines(filename).ToArray();
                var part1 = instance.Part1(input);
                Console.WriteLine($"Part 1: {part1}");
            } 
            catch (NotImplementedException)
            {
                Console.WriteLine("Part 1 is not implemented.");
            }

            try
            {
                var input = File.ReadLines(filename).ToArray();
                var part2 = instance.Part2(input);
                Console.WriteLine($"Part 2: {part2}");
            }
            catch (NotImplementedException)
            {
                Console.WriteLine("Part 2 is not implemented.");
            }
        }
    }
}
