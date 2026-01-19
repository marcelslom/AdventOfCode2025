
namespace Days
{
    public class Day10 : ISolution
    {
        public string Part1(string[] input)
        {
            var machines = input.Select(x => Machine.FromString(x)).ToList();

            var result = 0;
            foreach (var machine in machines)
            {
                var buttons = TryToLightUpMachine(machine);
                result += buttons.Count;
            }

            return result.ToString();
        }

        private static List<Button> TryToLightUpMachine(Machine machine)
        {
            foreach (var sequence in GetButtonsSequence(machine.Buttons))
            {
                foreach (var button in sequence)
                {
                    machine.PressButton(button);
                }

                if (machine.MachineIsLightUp())
                {
                    return sequence;
                }

                machine.ResetLights();
            }

            return null;
        }

        private static IEnumerable<List<Button>> GetButtonsSequence(List<Button> buttons)
        {
            for (var length = 1; length <= buttons.Count; length++)
            {
                foreach (var item in GetButtonsSequenceOfLength(buttons, length, 0))
                {
                    yield return item;
                }
            }
        }

        private static IEnumerable<List<Button>> GetButtonsSequenceOfLength(List<Button> buttons, int length, int offset)
        {
            if (length == 1)
            {
                for (var i = offset; i < buttons.Count; i++)
                {
                    yield return new List<Button> { buttons[i] };
                }
                yield break;
            }

            for (var i = offset; i < buttons.Count; i++)
            {
                foreach (var subSequence in GetButtonsSequenceOfLength(buttons, length - 1, i + 1))
                {
                    var sequence = new List<Button> { buttons[i] };
                    sequence.AddRange(subSequence);
                    yield return sequence;
                }
            }
        }

        public string Part2(string[] input)
        {
            var machines = input.Select(Machine.FromString).ToList();
            var result = 0L;
            for (var i = 0; i < machines.Count; i++)
            {
                var machine = machines[i];
                result += TheFewestPressesForMachine(machine);
            }

            return result.ToString();
        }
      
        private static int TheFewestPressesForMachine(Machine machine)
        {
            var matrix = CreateMatrix(machine);
            var originalMatrix = CopyMatrix(matrix);
            var freeVariableIndexes = GaussianElimination(matrix);
            matrix = RemoveEmptyRows(matrix);
            GaussJordan(matrix);
            var fixedVariables = FillFixedVariables(matrix, freeVariableIndexes);
            if (fixedVariables.Where(x => x.HasValue).Any(x => !IsInteger(x.Value)))
            {
                throw new Exception();
            }

            var fixedSum = fixedVariables.Select(x => Convert.ToInt32(x)).Sum();
            if (freeVariableIndexes.Count == 0)
            {
                return fixedSum;
            }
            else
            {
                var bounds = FindFreeVariableBounds(originalMatrix, freeVariableIndexes);
                var dependentCalculators = CreateDependentVariableCalculators(matrix, freeVariableIndexes);

                var min = int.MaxValue;
                foreach (var set in FreeVariablesSets(bounds, matrix[0].Length - 1))
                {
                    var dependentSum = 0;
                    var dependentVariablesAreValid = true;
                    foreach(var calc in dependentCalculators)
                    {
                        var d = calc.Calculate(set);
                        if (!d.correct)
                        {
                            dependentVariablesAreValid = false;
                            break;
                        }
                        dependentSum += d.result;
                    }

                    if (!dependentVariablesAreValid)
                    {
                        continue;
                    }

                    var sum = fixedSum + dependentSum + set.Sum(x => Convert.ToInt32(x));
                    min = Math.Min(min, sum);
                }

                return min;
            }
        }

        private static List<DependentVariableCalculator> CreateDependentVariableCalculators(double[][] matrix, List<int> freeVariableIndexes)
        {
            var result = new List<DependentVariableCalculator>();
            for (var row = 0; row < matrix.Length; row++)
            {
                if (!freeVariableIndexes.Any(x => !IsNear(matrix[row][x], 0)))
                {
                    continue;
                }
                var calc = new DependentVariableCalculator();
                var pivot = matrix[row].Index().First(x => !IsNear(x.Item, 0));
                calc.Index = pivot.Index;
                calc.Constant = matrix[row][^1];
                foreach (var freeVarIndex in freeVariableIndexes)
                {
                    if (IsNear(matrix[row][freeVarIndex], 0))
                    {
                        continue;
                    }

                    calc.FreeCoefficients[freeVarIndex] = matrix[row][freeVarIndex];
                }

                result.Add(calc);
            }

            return result;
        }

        private static double?[] FillFixedVariables(double[][] matrix, List<int> freeVariableIndexes)
        {
            var fixedVariables = new double?[matrix[0].Length - 1];
            for (var row = 0; row < matrix.Length; row++)
            {
                if (freeVariableIndexes.Any(x => !IsNear(matrix[row][x], 0)))
                {
                    continue;
                }

                var pivot = matrix[row].Index().First(x => !IsNear(x.Item, 0));
                fixedVariables[pivot.Index] = matrix[row][^1];
            }

            return fixedVariables;
        }

        private static IEnumerable<double?[]> FreeVariablesSets(List<FreeVariableBounds> bounds, int resultLength)
        {
            var maxes = new double?[bounds.Max(x => x.Index) + 1];
            var mins = new double?[bounds.Max(x => x.Index) + 1];
            var current = new double?[bounds.Max(x => x.Index) + 1];
            foreach (var x in bounds)
            {
                maxes[x.Index] = x.Max;
                mins[x.Index] = x.Min;
                current[x.Index] = x.Min;
            }

            var numberOfSets = bounds.Select(x => (int)x.Max - (int)x.Min + 1).Aggregate(1, (x, y) => x * y);
            for(var i = 0; i < numberOfSets; i++)
            {
                yield return current;

                for (var j = 0; j < current.Length; j++)
                {
                    if (current[j] == null)
                    {
                        continue;
                    }

                    current[j] += 1;
                    if (current[j] <= maxes[j])
                    {
                        break;
                    }

                    current[j] = mins[j];
                }
            }
        }

        private static List<FreeVariableBounds> FindFreeVariableBounds(double[][] matrix, List<int> freeVariableIndexes)
        {
            var bounds = new List<FreeVariableBounds>();
            foreach (var varIndex in freeVariableIndexes)
            {
                var rowsToProcess = matrix
                    .Where(row => row[varIndex] != 0); ;

                var minBound = 0.0;
                var maxBound = double.MaxValue;

                foreach (var row in rowsToProcess)
                {
                    if (row[varIndex] > 0)
                    {
                        maxBound = Math.Min(maxBound, row[^1] / row[varIndex]);
                    }
                    else
                    {
                        minBound = Math.Max(minBound, row[^1] / row[varIndex]);
                    }
                }

                if (maxBound == double.MaxValue)
                {
                    throw new Exception($"Max bound for '{varIndex}' variable index not found.");
                }

                bounds.Add(new FreeVariableBounds(varIndex, minBound, maxBound));
            }

            return bounds;
        }

        private static bool IsInteger(double d, double epsilon = 0.000001d)
        {
            try
            {
                if (d < -epsilon)
                {
                    return false;
                }

                var integer = Convert.ToInt32(d);
                return IsNear(d, integer, epsilon);
            } 
            catch
            {
                return false; 
            }
        }

        private static bool IsNear(double a, double b, double epsilon = 0.000001d)
        {
            return Math.Abs(a - b) <= epsilon;
        }

        private static double[][] CopyMatrix(double[][] matrix)
        {
            var copy = new double[matrix.Length][];
            for (var i = 0; i < matrix.Length; i++)
            {
                copy[i] = CopyRow(matrix[i]);
            }

            return copy;
        }

        private static double[] CopyRow(double[] row)
        {
            var copy = new double[row.Length];
            Array.Copy(row, copy, row.Length);
            return copy;
        }

        private static double[][] RemoveEmptyRows(double[][] matrix)
        {
            return [.. matrix.Where(x => x != null)];
        }

        private static double[][] CreateMatrix(Machine machine)
        {
            var matrix = new double[machine.DesiredJoltages.Length][];
            for (var i = 0; i < matrix.Length; i++)
            {
                var row = new double[machine.Buttons.Count + 1];
                row[^1] = machine.DesiredJoltages[i];
                for (var j = 0; j < machine.Buttons.Count; j++)
                {
                    row[j] = machine.Buttons[j].Toggles.Contains(i) ? 1 : 0;
                }

                matrix[i] = row;
            }

            return matrix;
        }

        private static void GaussJordan(double[][] matrix)
        {
            var rows = matrix.Length;
            var columns = matrix[0].Length;

            for (var row = rows - 1; row >= 0; row--)
            {
                var pivot = matrix[row].Index().First(x => !IsNear(x.Item, 0));
                for (var k = 0; k < row; k++)
                {
                    if (IsNear(matrix[k][pivot.Index], 0))
                    {
                        continue;
                    }

                    var multiplier = matrix[k][pivot.Index] / matrix[row][pivot.Index];
                    for (var column = 0; column < columns; column++)
                    {
                        matrix[k][column] = matrix[k][column] - matrix[row][column] * multiplier;
                    }
                }
            }
        }

        private static List<int> GaussianElimination(double[][] matrix)
        {
            var freeVariableIndexes = new List<int>();
            var rows = matrix.Length;
            var columns = matrix[0].Length;
            var lastColumn = -1;

            for(var row = 0; row < rows; row++)
            {
                for (var column = lastColumn + 1; column < columns; column++)
                {
                    var allZeroes = true;
                    var pivotRow = -1;
                    var pivotValue = Math.Abs(matrix[row][column]);
                    for (var i = row; i < rows; i++)
                    {
                        var abs = Math.Abs(matrix[i][column]);
                        if (!IsNear(abs, 0))
                        {
                            allZeroes = false;
                        }

                        if (abs > pivotValue)
                        {
                            pivotRow = i;
                            pivotValue = abs;
                        }
                    }

                    if (allZeroes)
                    {
                        freeVariableIndexes.Add(column);
                        continue;
                    }

                    if (pivotRow != -1)
                    {
                        (matrix[pivotRow], matrix[row]) = (matrix[row], matrix[pivotRow]);
                    }

                    if (matrix[row][column] != 1)
                    {
                        var factor = 1d / matrix[row][column];
                        for (var i = 0; i < columns; i++)
                        {
                            matrix[row][i] *= factor;
                        }
                    }

                    for (var nextRow = row + 1; nextRow < rows; nextRow++)
                    {
                        if (IsNear(matrix[nextRow][column], 0))
                        {
                            continue;
                        }

                        var factor = matrix[nextRow][column] / matrix[row][column];
                        for (var i = column; i < columns; i++)
                        {
                            matrix[nextRow][i] = matrix[nextRow][i] - factor * matrix[row][i];
                        }
                    }

                    lastColumn = column;
                    break;
                }

                for (var i = rows - 1; i >= 0; i--)
                {
                    var zeroes = matrix[i].SkipLast(1).All(x => IsNear(x, 0));
                    if (zeroes)
                    {
                        if (IsNear(matrix[i][columns - 1], 0))
                        {
                            RemoveRow(matrix, rows, i);
                            rows -= 1;
                        }
                        else
                        {
                            throw new Exception("No solution");
                        }
                    }
                }

                if (row == rows - 1 && lastColumn < columns - 2)
                {
                    freeVariableIndexes.AddRange(Enumerable.Range(lastColumn + 1, columns - 2 - lastColumn));
                }
            }

            return freeVariableIndexes;
        }

        private static void RemoveRow(double[][] matrix, int rows, int row)
        {
            for (var i = row; i < rows - 1; i++)
            {
                matrix[i] = matrix[i + 1];
            }

            matrix[rows - 1] = null;
        }

        private class DependentVariableCalculator
        {
            public int Index { get; set; }
            public double Constant { get;set; }
            public Dictionary<int, double> FreeCoefficients { get; set; } = [];

            public (bool correct, int result) Calculate(double?[] freeVariables)
            {
                var result = Constant;
                foreach(var pair in FreeCoefficients)
                {
                    var free = pair.Value * freeVariables[pair.Key].Value;
                    result -= free;
                }

                if (!IsInteger(result))
                {
                    return (false, 0);
                }

                return (true, Convert.ToInt32(result));
            }
        }

        private record struct FreeVariableBounds(int Index, double Min, double Max)
        {
        }

        private struct Button
        {
            public int[] Toggles { get; }

            public Button(int[] toggles)
            {
                Toggles = toggles;
            }

            public override readonly string ToString()
            {
                return $"({string.Join(",", Toggles)})";
            }
        }
        
        private class Machine
        {

            public Machine(bool[] desiredIndicators, List<Button> buttons, int[] desiredJoltages)
            {
                DesiredIndicators = desiredIndicators;
                Buttons = buttons;
                DesiredJoltages = desiredJoltages;
                Indicators = new bool[desiredIndicators.Length];
            }

            public bool[] DesiredIndicators { get; }
            public int[] DesiredJoltages { get; }
            public List<Button> Buttons { get; }
            public bool[] Indicators { get; private set; }

            public void PressButton(Button button)
            {
                foreach (var position in button.Toggles)
                {
                    Indicators[position] = !Indicators[position];
                }            
            }

            public bool MachineIsLightUp()
            {
                for (var i = 0; i < Indicators.Length; i++)
                {
                    if (Indicators[i] != DesiredIndicators[i])
                    {
                        return false; 
                    }
                }

                return true;
            }

            public void ResetLights()
            {
                Indicators = new bool[DesiredIndicators.Length];
            }

            public static Machine FromString(string input)
            {
                var splitted = input.Split(' ');
                var indicators = splitted[0]
                    .Skip(1)
                    .SkipLast(1)
                    .Select(x => x == '#')
                    .ToArray();
                var buttons = splitted
                    .Skip(1)
                    .SkipLast(1)
                    .Select(x => x.Replace("(", "").Replace(")", ""))
                    .Select(x => x.Split(","))
                    .Select(x => x.Select(int.Parse).ToArray())
                    .Select(x => new Button(x))
                    .ToList();
                var joltages = splitted
                    .Last()
                    .Replace("{", "")
                    .Replace("}", "")
                    .Split(",")
                    .Select(int.Parse)
                    .ToArray();
                
                return new Machine(indicators, buttons, joltages);
            }
        }

    }
}
