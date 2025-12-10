namespace Days
{
    public class Day9 : ISolution
    {
        public string Part1(string[] input)
        {
            var tiles = ParseInput(input);
            
            var maxArea = 0L;
            foreach (var tile in tiles)
            {
                foreach (var secondTile in tiles)
                {
                    if (tile == secondTile )
                    {
                        continue;
                    }

                    var area = Area(tile, secondTile);
                    if (area > maxArea)
                    {
                        maxArea = area;
                    }
                }
            }

            return maxArea.ToString();
        }

        static long Area(Tile first, Tile second)
        {
            return ((long)Math.Abs(first.X - second.X) + 1) * (Math.Abs(first.Y - second.Y) + 1);
        }

        static List<Tile> ParseInput(string[] input)
        {
            return input
                .Select(x => x.Split(','))
                .Select(x => x.Select(xx => int.Parse(xx)).ToArray())
                .Select(x => new Tile(x[0], x[1]))
                .ToList();
        }

        static Dictionary<(Tile, Tile), long> GetAreas(List<Tile> tiles)
        {
            var areas = new Dictionary<(Tile, Tile), long>();
            foreach (var tile in tiles)
            {
                foreach (var secondTile in tiles)
                {
                    if (tile == secondTile)
                    {
                        continue;
                    }

                    if (areas.ContainsKey((secondTile, tile)))
                    {
                        continue;
                    }

                    areas[(tile, secondTile)] = Area(tile, secondTile);
                }
            }
            return areas;
        }

        static List<Edge> GetEdges(List<Tile> tiles)
        {
            var edges = new List<Edge>();
            for (var i = 0; i < tiles.Count - 1; i++)
            {
                edges.Add(new Edge(tiles[i], tiles[i + 1]));
            }

            edges.Add(new Edge(tiles[^1], tiles[0]));

            return edges;
        }

        static bool ForeignEdgeInRectange(Tile first, Tile second, List<Edge> edges)
        {
            if (Math.Abs(first.X - second.X) < 2 || Math.Abs(first.Y - second.Y) < 2)
            {
                return false;
            }

            var minX = Math.Min(first.X, second.X) + 1;
            var maxX = Math.Max(first.X, second.X) - 1;
            var minY = Math.Min(first.Y, second.Y) + 1;
            var maxY = Math.Max(first.Y, second.Y) - 1;

            foreach (var edge in edges)
            {
                if (edge.Orientation == Orientation.Horizontal)
                {
                    if (edge.First.Y < minY || edge.First.Y > maxY)
                    {
                        continue;
                    }

                    if (edge.First.X < minX && edge.Second.X < minX || edge.First.X > maxX && edge.Second.X > maxX)
                    {
                        continue;
                    }

                    return true;
                }
                else
                {
                    if (edge.First.X < minX || edge.First.X > maxX)
                    {
                        continue;
                    }

                    if (edge.First.Y < minY && edge.Second.Y < minY || edge.First.Y > maxY && edge.Second.Y > maxY)
                    {
                        continue;
                    }

                    return true;
                }
            }

            return false;
        }

        public string Part2(string[] input)
        {
            var tiles = ParseInput(input);
            var areas = GetAreas(tiles);
            var edges = GetEdges(tiles);

            var sorted = areas.OrderByDescending(x => x.Value).ToList();
            var result = sorted.First(x => !ForeignEdgeInRectange(x.Key.Item1, x.Key.Item2, edges));

            return result.Value.ToString();
        }
    }

    record struct Tile(int X, int Y);

    record struct Edge(Tile First, Tile Second, Orientation Orientation)
    {
        public Edge(Tile First, Tile Second) : this(First, Second, First.X == Second.X ? Orientation.Vertical : First.Y == Second.Y ? Orientation.Horizontal : throw new NotSupportedException())
        {
        }
    }

    internal enum Orientation
    {
        Horizontal, Vertical
    }
}
