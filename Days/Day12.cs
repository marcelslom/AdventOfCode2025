using System;
using System.Collections.Generic;
using System.Text;

namespace Days
{
    public class Day12 : ISolution
    {
        public string Part1(string[] input)
        {
            var (shapes, regions) = ParseInput(input);
            var result = 0;
            foreach (var region in regions)
            {
                var area = region
                    .ShapeCounts
                    .Index()
                    .Where(x => x.Item > 0)
                    .Select(x => shapes[x.Index].GetArea() * x.Item)
                    .Sum();
                if (area > region.X * region.Y)
                {
                    continue;
                }

                var numberOfGifts = region.ShapeCounts.Sum();
                var numberOfRegions = (region.X / 3) * (region.Y / 3);

                if (numberOfRegions >= numberOfGifts)
                {
                    result++;
                    continue;
                }

                throw new Exception();
            }

            return result.ToString();
        }

        private (List<Shape> shapes, List<Region> regions) ParseInput(string[] input)
        {
            List<Shape> shapes = [];
            List<Region> regions = [];
            var shapeIndex = 0;
            var shapeValue = new List<bool[]>();
            for (var i = 0; i < input.Length; i++)
            {
                var line = input[i];
                if (string.IsNullOrWhiteSpace(line))
                {
                    shapes.Add(new Shape(shapeIndex, shapeValue.ToArray()));
                    shapeValue = [];
                    continue;
                }
                
                if (line.EndsWith(':'))
                {
                    shapeIndex = int.Parse(line[..^1]);
                }
                else if(line.StartsWith('.') ||  line.StartsWith('#'))
                {
                    shapeValue.Add([.. line.Select(x => x == '#')]);
                }
                else
                {
                    var splitted = line.Split(':');
                    var dims = splitted[0].Split('x').Select(int.Parse).ToArray();
                    var counts = splitted[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                    regions.Add(new Region(dims[0], dims[1], counts));
                }
            }

            return (shapes, regions);
        }

        public string Part2(string[] input)
        {
            throw new NotImplementedException();
        }

        private record Region(int X, int Y, int[] ShapeCounts);

        private class Shape
        {
            public static readonly List<(Rotation, bool)> TransformationKeys = Enum.GetValues<Rotation>().Select(x => (x, false)).Concat(Enum.GetValues<Rotation>().Select(x => (x, true))).ToList();
            public int Index { get; }
            public bool[][] Value { get; }
            public Dictionary<(Rotation, bool), bool[][]?> Transformations { get; }

            public bool[][]? GetTransformation(int index)
            {
                return Transformations[TransformationKeys[index]];
            }

            public Shape(int index, bool[][] value)
            {
                Index = index;
                Value = value;
                Transformations = GenerateAllTransformations();
            }

            public int GetArea()
            {
                return Value.SelectMany(x => x).Count(x => x);
            }

            private Dictionary<(Rotation, bool), bool[][]?> GenerateAllTransformations()
            {
                var result = new Dictionary<(Rotation, bool), bool[][]?>();
                foreach (var rotation in Enum.GetValues<Rotation>())
                {
                    var transformed = GetTransformedValue(rotation, false);
                    if(!result.Any(x => AreEqual(x.Value, transformed))) 
                    {
                        result[(rotation, false)] = transformed;
                    }
                    else
                    {
                        result[(rotation, false)] = null;
                    }

                    transformed = GetTransformedValue(rotation, true);
                    if (!result.Any(x => AreEqual(x.Value, transformed)))
                    {
                        result[(rotation, true)] = transformed;
                    }
                    else
                    {
                        result[(rotation, true)] = null;
                    }
                }

                return result;
            }

            private bool AreEqual(bool[][]? first, bool[][]? second)
            {
                if (first == null || second == null)
                {
                    return false;
                }

                if (first.Length != second.Length)
                {
                    return false;
                }

                if (first[0].Length != second[0].Length)
                {
                    return false;
                }

                for (var i = 0; i < first.Length; i++)
                {
                    for (var j = 0; j < first[0].Length; j++)
                    {
                        if (first[i][j] != second[i][j])
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            private bool[][] GetTransformedValue(Rotation rotation, bool flipped)
            {
                var result = Value;
                if (flipped)
                {
                    result = FlipValue(result);
                    result = RotateValue(result, rotation);
                }
                else
                {
                    result = RotateValue(result, rotation);
                }
                return result;
            }

            private bool[][] FlipValue(bool[][] value)
            {
                var result = Enumerable.Range(0, value.Length).Select(x => new bool[value[0].Length]).ToArray();
                for (var oldY = 0; oldY < value.Length; oldY++)
                {
                    for (var oldX = 0; oldX < value[oldY].Length; oldX++)
                    {
                        result[oldY][value[0].Length - oldX - 1] = value[oldY][oldX];
                    }
                }
                return result;
            }

            private bool[][] RotateValue(bool[][] value, Rotation rotation)
            {
                switch (rotation) 
                {
                    case Rotation.Degrees0:
                    {
                        return value;
                    }
                    case Rotation.Degrees90:
                    {
                        var rotated = Enumerable.Range(0, value[0].Length).Select(x => new bool[value.Length]).ToArray();
                        for(var oldY =  0; oldY < value.Length; oldY++)
                        {
                            for(var oldX = 0; oldX < value[oldY].Length; oldX++)
                            {
                                rotated[oldX][value.Length - oldY - 1] = value[oldY][oldX];
                            }
                        }

                        return rotated;
                    }
                    case Rotation.Degrees180:
                    {
                        var rotated = Enumerable.Range(0, value.Length).Select(x => new bool[value[0].Length]).ToArray();
                        for (var oldY = 0; oldY < value.Length; oldY++)
                        {
                            for (var oldX = 0; oldX < value[oldY].Length; oldX++)
                            {
                                rotated[value.Length - oldY - 1][value[0].Length - oldX - 1] = value[oldY][oldX];
                            }
                        }

                        return rotated;
                    }
                    case Rotation.Degrees270:
                    {
                        var rotated = Enumerable.Range(0, value[0].Length).Select(x => new bool[value.Length]).ToArray();
                        for (var oldY = 0; oldY < value.Length; oldY++)
                        {
                            for (var oldX = 0; oldX < value[oldY].Length; oldX++)
                            {
                                rotated[value[0].Length - oldX - 1][oldY] = value[oldY][oldX];
                            }
                        }

                        return rotated;
                    }
                    default:
                    {
                        throw new NotImplementedException();
                    }
                };
            }
        }

        private enum Rotation
        {
            Degrees0,
            Degrees90,
            Degrees180,
            Degrees270
        }

        private record Gift(Shape Shape)
        {

            public int X { get; set; } = -1;
            public int Y { get; set; } = -1;
            private int transformationIndex;

            public void Reset()
            {
                X = -1;
                Y = -1;
                transformationIndex = 0;
            }

            public bool[][] CurrentShape => Shape.GetTransformation(transformationIndex)!;

            public bool Transform()
            {
                for(var i = transformationIndex + 1; i < Shape.TransformationKeys.Count; i++)
                {
                    if (Shape.GetTransformation(i) != null)
                    {
                        transformationIndex = i;
                        return true;
                    }
                }

                transformationIndex = 0;
                return false;
            }
        }
    }
}
