using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectromagneticAlgorithm
{
    public static class AlgorithmUtils
    {
        private static Random rng = new Random();

        internal class Map<T1, T2> // Źródło: https://stackoverflow.com/questions/10966331/two-way-bidirectional-dictionary-in-c
        {
            private Dictionary<T1, T2> _forward = new Dictionary<T1, T2>();
            private Dictionary<T2, T1> _reverse = new Dictionary<T2, T1>();

            internal Map()
            {
                this.Forward = new Indexer<T1, T2>(_forward);
                this.Reverse = new Indexer<T2, T1>(_reverse);
            }

            internal class Indexer<T3, T4>
            {
                private Dictionary<T3, T4> _dictionary;
                public Indexer(Dictionary<T3, T4> dictionary)
                {
                    _dictionary = dictionary;
                }
                public T4 this[T3 index]
                {
                    get { return _dictionary[index]; }
                    set { _dictionary[index] = value; }
                }

                // My addition
                public List<T3> GetKeys()
                {
                    return new List<T3>(_dictionary.Keys);
                }
            }

            public void Add(T1 t1, T2 t2)
            {
                _forward.Add(t1, t2);
                _reverse.Add(t2, t1);
            }

            internal Indexer<T1, T2> Forward { get; private set; }
            internal Indexer<T2, T1> Reverse { get; private set; }
        }

        public static void ValidatePermutation(List<int> repr, int solutionLength)
        {
            if (repr.Count != solutionLength) // Is solution the same length?
                throw new AlgorithmUtils.InvalidSolutionException($"Podana wartość ma złą długość ({repr.Count}), powinna mieć długość {solutionLength}");

            if (repr.Count != repr.Distinct().Count()) // Are there no duplicates?
                throw new AlgorithmUtils.InvalidSolutionException($"Rozwiązanie ma duplikaty: {repr}");

            if (repr.Min() != 0 || repr.Max() != solutionLength - 1) // Are the values proper?
                throw new AlgorithmUtils.InvalidSolutionException($"Rozwiązanie ma niepoprawne wartości: {repr}");
        }

        public static T ValidateSolutionType<T>(ISolution solution)
        {
            if (solution is T)
                return (T)solution;
            else
                throw new MismatchedSolutionException();
        }

        public static void Shuffle<T>(this IList<T> list) // Źródło: https://stackoverflow.com/questions/273313/randomize-a-listt
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static string EscapeIt(string value)
        {
            var builder = new StringBuilder();
            foreach (var cur in value)
            {
                switch (cur)
                {
                    case '\t':
                        builder.Append(@"\t");
                        break;
                    case '\r':
                        builder.Append(@"\r");
                        break;
                    case '\n':
                        builder.Append(@"\n");
                        break;
                    // etc ...
                    default:
                        builder.Append(cur);
                        break;
                }
            }
            return builder.ToString();
        } // Źródło: https://stackoverflow.com/questions/9418517/how-to-force-console-writeline-to-print-verbatim-strings

        public static void PrintMatrix<T>(T[,] matrix) // Źródło: https://stackoverflow.com/questions/12826760/printing-2d-array-in-matrix-format
        {
            int rowLength = matrix.GetLength(0);
            int colLength = matrix.GetLength(1);

            for (int i = 0; i < rowLength; i++)
            {
                for (int j = 0; j < colLength; j++)
                {
                    Console.Write(string.Format("{0} ", matrix[i, j]));
                }
                Console.Write(Environment.NewLine + Environment.NewLine);
            }
        }

        public static T[] ChooseRandom<T>(this T[] arr, int n) // Źródło: https://www.reddit.com/r/dotnet/comments/nfwtbg/what_is_the_net_way_of_choosing_n_random_elements/
        {
            var rng = new Random();

            var sample = Enumerable
                .Range(0, n)
                .Select(x => rng.Next(0, 1 + arr.Length - n))
                .OrderBy(x => x)
                .Select((x, i) => arr[x + i])
                .ToArray();

            return sample;
        }

        // Exceptions
        public class InvalidSolutionException : Exception
        {
            public InvalidSolutionException(string message) : base(message) { }
        }

        public class MismatchedSolutionException : Exception { } // Kiedy próbujemy robić crossover różnych typów rozwiązań

        public class SolutionNotInitializedException : Exception { }
    }
}
