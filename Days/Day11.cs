using System.Text;

namespace Days
{
    public class Day11 : ISolution
    {
        public string Part1(string[] input)
        {
            var servers = ParseInput(input);
            var inputEdges = FindInputEdges(servers);
            var sorted = TopologicalSort(inputEdges);
            return CountPaths(inputEdges, sorted, "you", "out").ToString();
        }

        private static long CountPaths(Dictionary<string, string[]> inputEdges, List<string> sorted, string start, string end, long initialCount = 1)
        {
            var startIndex = sorted.IndexOf(start);
            var nodeCounters = new Dictionary<string, long>
            {
                [start] = initialCount
            };

            foreach (var node in sorted.Skip(startIndex + 1))
            {
                var pathsCount = inputEdges[node].Sum(x => nodeCounters.TryGetValue(x, out var c) ? c : 0);
                nodeCounters[node] = pathsCount;
                if (node == end)
                {
                    break;
                }
            }

            return nodeCounters[end];
        }

        private Dictionary<string, string[]> ParseInput(string[] input)
        {
            var result = new Dictionary<string, string[]>();
            foreach (var splitted in input.Select(x => x.Split(": ")))
            {
                result[splitted[0]] = splitted[1].Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            }

            return result;
        }

        public string Part2(string[] input)
        {
            var servers = ParseInput(input);
            var inputEdges = FindInputEdges(servers);
            var sorted = TopologicalSort(inputEdges);
            if (sorted.IndexOf("fft") < sorted.IndexOf("dac"))
            {
                var first = CountPaths(inputEdges, sorted, "svr", "fft");
                var second = CountPaths(inputEdges, sorted, "fft", "dac", first);
                return CountPaths(inputEdges, sorted, "dac", "out", second).ToString();
            } 
            else
            {
                var first = CountPaths(inputEdges, sorted, "svr", "dac");
                var second = CountPaths(inputEdges, sorted, "dac", "fft", first);
                return CountPaths(inputEdges, sorted, "fft", "out", second).ToString();
            }
        }

        private Dictionary<string, string[]> FindInputEdges(Dictionary<string, string[]> servers)
        {
            var result = new Dictionary<string, string[]>();
            foreach (var server in servers.Append(new KeyValuePair<string, string[]>("out", [])))
            {
                var edges = servers.Where(x => x.Value.Any(x => x == server.Key)).Select(x => x.Key).ToArray();
                result[server.Key] = edges;
            }
            return result;
        }

        private List<string> TopologicalSort(Dictionary<string, string[]> inputEdges)
        {
            var copy = new List<(string id, List<string> edges)>();
            foreach (var edge in inputEdges)
            {
                copy.Add((edge.Key, edge.Value.ToList()));
            }

            var result = new List<string>();
            for (var i = 0; i < inputEdges.Count; i++)
            {
                var element = copy.First(x => x.edges.Count == 0);
                copy.Remove(element);
                foreach (var (id, edges) in copy)
                {
                    edges.Remove(element.id);
                }
                result.Add(element.id);
            }

            return result;
        }
    }
}
