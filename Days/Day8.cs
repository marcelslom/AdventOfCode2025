namespace Days
{
    public class Day8 : ISolution
    {
        public string Part1(string[] input)
        {
            var junctions = ParseInput(input);
            var distances = CalculateDistances(junctions);
            var junctionsToConnect = distances.OrderBy(x  => x.Value).Select(x => x.Key).ToList();
            var circuits = new List<HashSet<Junction>>();

            foreach (var pair in junctionsToConnect.Take(1000))
            {
                var existingCircuit1Index = circuits.TakeWhile(c => !c.Contains(pair.Item1)).Count();
                var existingCircuit2Index = circuits.TakeWhile(c => !c.Contains(pair.Item2)).Count();

                if (existingCircuit1Index < circuits.Count && existingCircuit1Index == existingCircuit2Index)
                {
                    continue;
                }

                if (existingCircuit1Index == circuits.Count && existingCircuit2Index == circuits.Count)
                {
                    circuits.Add([pair.Item1, pair.Item2]);
                    continue;
                }

                if (existingCircuit1Index < circuits.Count && existingCircuit2Index < circuits.Count)
                {
                    circuits[existingCircuit1Index].UnionWith(circuits[existingCircuit2Index]);
                    circuits.RemoveAt(existingCircuit2Index);
                    continue;
                }

                if (existingCircuit1Index < circuits.Count)
                {
                    circuits[existingCircuit1Index].Add(pair.Item2);
                    continue;
                }

                if (existingCircuit2Index < circuits.Count)
                {
                    circuits[existingCircuit2Index].Add(pair.Item1);
                    continue;
                }
            }

            var result = circuits.Select(x => x.Count).OrderDescending().Take(3).Aggregate(1, (x, y) => x * y);
            return result.ToString();
        }

        private static Dictionary<(Junction, Junction), double> CalculateDistances(List<Junction> junctions)
        {
            var distances = new Dictionary<(Junction, Junction), double>();
            foreach (var junction in junctions)
            {
                foreach (var other in junctions)
                {
                    if (junction == other)
                    {
                        continue;
                    }

                    if (distances.ContainsKey((other, junction)))
                    {
                        continue;
                    }

                    distances[(junction, other)] = junction.GetDistance(other);
                }
            }

            return distances;
        }

        static List<Junction> ParseInput(string[] input)
        {
            return input.Select(x => x.Split(',')).Select(x => x.Select(xx => int.Parse(xx)).ToArray()).Select(x => new Junction(x[0], x[1], x[2])).ToList();
        }

        public string Part2(string[] input)
        {
            var lastPair = SimulatePart2(input);
            var result = (long)lastPair.Value.Item1.X * lastPair.Value.Item2.X;
            return result.ToString();
        }

        static (Junction, Junction)? SimulatePart2(string[] input)
        {
            var junctions = ParseInput(input);
            var distances = CalculateDistances(junctions);
            var junctionsToConnect = distances.OrderBy(x => x.Value).Select(x => x.Key).ToList();
            var circuits = new List<HashSet<Junction>>();

            for (var i = 0; i < junctionsToConnect.Count; i++)
            {
                if (circuits.Count == 1 && circuits[0].Count == junctions.Count)
                {
                    return junctionsToConnect[i - 1];
                }

                var pair = junctionsToConnect[i];

                var existingCircuit1Index = circuits.TakeWhile(c => !c.Contains(pair.Item1)).Count();
                var existingCircuit2Index = circuits.TakeWhile(c => !c.Contains(pair.Item2)).Count();

                if (existingCircuit1Index < circuits.Count && existingCircuit1Index == existingCircuit2Index)
                {
                    continue;
                }

                if (existingCircuit1Index == circuits.Count && existingCircuit2Index == circuits.Count)
                {
                    circuits.Add([pair.Item1, pair.Item2]);
                    continue;
                }

                if (existingCircuit1Index < circuits.Count && existingCircuit2Index < circuits.Count)
                {
                    circuits[existingCircuit1Index].UnionWith(circuits[existingCircuit2Index]);
                    circuits.RemoveAt(existingCircuit2Index);
                    continue;
                }

                if (existingCircuit1Index < circuits.Count)
                {
                    circuits[existingCircuit1Index].Add(pair.Item2);
                    continue;
                }

                if (existingCircuit2Index < circuits.Count)
                {
                    circuits[existingCircuit2Index].Add(pair.Item1);
                    continue;
                }
            }

            return null;
        }

        record struct Junction(int X, int Y, int Z)
        {
            public double GetDistance(Junction other)
            {
                return Math.Sqrt(Math.Pow(X - other.X, 2) + Math.Pow(Y - other.Y, 2) + Math.Pow(Z - other.Z, 2));
            }
        }
    }
}
