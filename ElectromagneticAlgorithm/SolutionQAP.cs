using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ElectromagneticAlgorithm
{
    public class SolutionQAP : ISolution
    {
        public static int solutionLength;
        protected static int maxMatchingRegion; // Maximum region of that PMX can crossover
        protected static int[,] facilityFlows;
        protected static int[,] locationDistances;

        protected List<int>? assignmentPermutation;
        //private int solutionLength;
        protected Random random = new();

        public virtual ISolution GetCopy()
        {
            SolutionQAP newSolution = new();
            newSolution.SetSolutionRepresentation(GetSolutionRepresentation());
            return newSolution;
        }

        public static void SetQAPData(int[,] flows, int[,] distances)
        {
            facilityFlows = flows;
            locationDistances = distances;
        }

        // Wczytywanie danych facility flows, location distances z pliku
        public static void SetQAPData(string filePath)
        {
            string content = File.ReadAllText(filePath);
            string[] splittedContent = content.Split("\n");
            solutionLength = int.Parse(splittedContent[0]);
            maxMatchingRegion = solutionLength / 2;
            //Console.WriteLine(solutionLength);
            //Console.WriteLine();

            facilityFlows = new int[solutionLength, solutionLength];
            locationDistances = new int[solutionLength, solutionLength];

            int lineCounter = 2;
            int i, j;

            i = 0;
            while (splittedContent[lineCounter] != string.Empty)
            {
                string newLine = splittedContent[lineCounter];

                string newValue = "";
                j = 0;
                foreach (char c in newLine)
                {
                    if (c == ' ' && newValue != "")
                    {
                        facilityFlows[i, j] = int.Parse(newValue);
                        j++;
                        newValue = "";
                    }
                    else if (c != ' ')
                        newValue += c;
                }

                if (newValue != "") facilityFlows[i, j] = int.Parse(newValue);

                i++;
                lineCounter++;
            }
            lineCounter++;

            i = 0;
            while (splittedContent[lineCounter] != string.Empty)
            {
                string newLine = splittedContent[lineCounter];

                string newValue = "";
                j = 0;
                foreach (char c in newLine)
                {
                    if (c == ' ' && newValue != "")
                    {
                        locationDistances[i, j] = int.Parse(newValue);
                        j++;
                        newValue = "";
                    }
                    else if (c != ' ')
                        newValue += c;
                }

                if (newValue != "") locationDistances[i, j] = int.Parse(newValue);

                i++;
                lineCounter++;
            }

            /*AlgorithmUtils.PrintMatrix(facilityFlows);
            Console.WriteLine("\n\nkaczka\n\n");
            AlgorithmUtils.PrintMatrix(locationDistances);
            Console.WriteLine();*/
        }

        public static double GetAverageCost()
        {
            int sumFij = 0, sumDkl = 0; //, sumFii = 0, sumDkk = 0;
            int n = solutionLength;

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i != j)
                    {
                        sumFij += facilityFlows[i, j];
                        sumDkl += locationDistances[i, j];
                    }
                    /*else
                    {
                        sumFii += facilityFlows[i, i];
                        sumDkk += locationDistances[i, i];
                    }*/
                }
            }

            double n_s = 1.0/(n * (n - 1));
            return n_s * sumFij * sumDkl; // + ((1.0/n) * sumFii * sumDkk);
        }


        //public SolutionQAP() : this(Enumerable.Range(0, solutionLength).ToList()) { }

        //private SolutionQAP(SolutionQAP solution) : this(solution.GetSolutionRepresentation()) { }


        public double GetCost()
        {
            int totalCost = 0;
            int n = solutionLength;

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    int facility1 = i;
                    int facility2 = j;
                    int location1 = assignmentPermutation[i];
                    int location2 = assignmentPermutation[j];

                    totalCost += facilityFlows[facility1, facility2] * locationDistances[location1, location2];
                }
            }

            return totalCost;
        }

        public virtual void PullTowardsSolution(ISolution secondSolution, double secondSolForce, ISolution[] neighbouringSolutions, double[] attractionForces, int k, bool exploration)
        {
            // Establish the mapping region randomly
            int l = random.Next(solutionLength);
            double forceRatio = secondSolForce / attractionForces.Sum();
            int addedRange = (int)Math.Ceiling(forceRatio * solutionLength);
            
            //if (addedRange == solutionLength - 1) Console.WriteLine("WoW");
            if (addedRange > maxMatchingRegion)
            {
                addedRange = maxMatchingRegion;
                //addedRange = solutionLength - 1;
                //Console.WriteLine(ToString());
                //Console.WriteLine(secondSolution.ToString());
            }
            //Console.WriteLine(addedRange);
            int r = (l + addedRange) % solutionLength;

            // PMX
            StandardPMX(secondSolution, l, r);
        }

        protected void StandardPMX(ISolution secondSolution, int l, int r)
        {
            //Console.WriteLine($"l = {l}, r = {r}");

            SolutionQAP solution2 = AlgorithmUtils.ValidateSolutionType<SolutionQAP>(secondSolution);

            // Get copies of solutions
            List<int> s1 = new(this.GetSolutionRepresentation());
            List<int> s2 = new(solution2.GetSolutionRepresentation());
            AlgorithmUtils.ValidatePermutation(s2, solutionLength);

            List<int> s1Copy = new(s1);
            int range = r - l;

            //if (range == 0) throw new Exception("Right side of the range must not be equal to the left side.");

            if (range == 0)
            {
                Console.WriteLine("Warning! Right side of the range must not be equal to the left side.");
                return;
            }

            // Swap a slice of s1 with slice of s2, and vice versa + Determine the mapping relationship
            AlgorithmUtils.Map<int, int> mappingRelationship = new();
            int n = l;
            while (n != r) // This will loop around if l > r (which can happen)
            {
                s1[n] = s2[n];
                s2[n] = s1Copy[n];
                mappingRelationship.Add(s1[n], s2[n]);

                n++;
                n %= solutionLength;
            }

            // Legalize solution with the mapping relationship
            List<int> forwardElements = mappingRelationship.Forward.GetKeys();
            List<int> reverseElements = mappingRelationship.Reverse.GetKeys();

            void LegalizeSolution(List<int> solutionRepr, int i, AlgorithmUtils.Map<int, int>.Indexer<int, int> mapping, List<int> mappingElements)
            {
                while (mappingElements.Contains(solutionRepr[i]))
                    solutionRepr[i] = mapping[solutionRepr[i]];
            }

            n = r;
            while (n != l) // This will loop around if l < r
            {
                LegalizeSolution(s1, n, mappingRelationship.Forward, forwardElements);
                //LegalizeSolution(s2, n, mappingRelationship.Reverse, reverseElements);
                n++;
                n %= solutionLength;
            }

            // Replace the solutions
            this.SetSolutionRepresentation(s1);
            //solution2.SetSolutionRepresentation(s2);
        }

        public virtual void RepelFromSolution(ISolution secondSolution, double secondSolForce, ISolution[] neighbouringSolutions, double[] attractionForces, int k, bool exploration)
        {
            int sameElementsSum = 0;
            for (int j = 0; j < neighbouringSolutions.Length; j++)
                sameElementsSum += solutionLength - (int)GetDistanceFromSolution(neighbouringSolutions[j]);

            int noElementsInSamePositions = solutionLength - (int)GetDistanceFromSolution(secondSolution);
            double forceRatio = secondSolForce / attractionForces.Sum();

            int noElements = Math.Min((int)Math.Ceiling(sameElementsSum * forceRatio), noElementsInSamePositions); // Wzór (4) praca WCH
            //Console.WriteLine($"Liczba zmienionych: {noElements}, max: {noElementsInSamePositions}");

            if (noElements <= 1) return; // No use shuffling 1 or 0 elements

            SolutionQAP solution2 = AlgorithmUtils.ValidateSolutionType<SolutionQAP>(secondSolution);

            // Get copies of solutions
            List<int> s1 = new(this.GetSolutionRepresentation());
            List<int> s2 = new(solution2.GetSolutionRepresentation());
            AlgorithmUtils.ValidatePermutation(s2, solutionLength);

            List<int> matchingElementsIndexes = new();
            for (int i = 0; i < solutionLength; i++)
            {
                if (s1[i] == s2[i])
                {
                    matchingElementsIndexes.Add(i);
                }
            }

            // We decide on elements we shuffle
            List<int> matchingElementsIndexesCopy = new List<int>(matchingElementsIndexes);
            bool badShuffle = true;
            while (badShuffle) // Shuffling until we get something that has no elements left on original positions
            {
                badShuffle = false;
                AlgorithmUtils.Shuffle(matchingElementsIndexes);
                for (int i = 0; i < matchingElementsIndexes.Count; i++)
                {
                    if (matchingElementsIndexes[i] == matchingElementsIndexesCopy[i])
                    {
                        //Console.WriteLine("Bad shuffle");
                        badShuffle = true;
                        matchingElementsIndexes = new List<int>(matchingElementsIndexesCopy);
                        break;
                    }
                }
            }
            //Console.WriteLine($"orig: {String.Join("; ", matchingElementsIndexesCopy)}, shuffle: {String.Join("; ", matchingElementsIndexes)}");

            List<int> indexesToShuffleUnsorted = matchingElementsIndexes.Take(noElements).ToList();
            List<int> indexesToShuffle = new List<int>(indexesToShuffleUnsorted);
            indexesToShuffle.Sort();

            // We use the random order we already have from before
            List<int> s1copy = new List<int>(s1);
            for (int i = 0; i < indexesToShuffle.Count; i++)
            {
                s1[indexesToShuffle[i]] = s1copy[indexesToShuffleUnsorted[i]];
            }

            this.SetSolutionRepresentation(s1);
        }

        public double GetDistanceFromSolution(ISolution solution)
        {
            SolutionQAP sol = AlgorithmUtils.ValidateSolutionType<SolutionQAP>(solution);

            List<int> s1 = new(this.GetSolutionRepresentation());
            List<int> s2 = new(sol.GetSolutionRepresentation());
            AlgorithmUtils.ValidatePermutation(s2, solutionLength);

            int hammingDistance = 0;
            for (int i = 0; i < solutionLength; i++)
            {
                if (s1[i] != s2[i]) hammingDistance++;
            }
            return hammingDistance;
        }

        /*public int GetSolutionLength()
        {
            return solutionLength;
        }*/

        public List<int> GetSolutionRepresentation()
        {
            return new List<int>(assignmentPermutation);
        }

        public void SetSolutionRepresentation<T>(T repr)
        {
            if (typeof(T) != typeof(List<int>)) throw new Exception("Wrong representation type.");
            List<int> repr2 = (List<int>)Convert.ChangeType(repr, typeof(List<int>));

            AlgorithmUtils.ValidatePermutation(repr2, solutionLength);

            assignmentPermutation = new List<int>(repr2);
        }

        public void ShuffleRepresentation(int? seed = null)
        {
            AlgorithmUtils.Shuffle(assignmentPermutation, seed);
        }

        public override string ToString()
        {
            StringBuilder sb = new(assignmentPermutation[0].ToString());
            for (int i = 1; i < assignmentPermutation.Count; i++)
            {
                sb.Append(", ");
                sb.Append(assignmentPermutation[i].ToString());
            }

            return sb.ToString();
        }

        public static double GetConditionalExpectedCost(int[] c)
        {
            List<int> Hlis = new(); List<int> Ulis = new();

            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] < int.MaxValue)
                    Hlis.Add(i);

                if (!c.Contains(i))
                    Ulis.Add(i);
            }

            // H -> fixed indexes, U -> loose values
            int[] H = Hlis.ToArray(); int[] U = Ulis.ToArray();

            int n = solutionLength;
            int k = H.Length;

            // If there is only one loose value we can just insert it and calculate the cost normally
            if (k == n - 1)
            {
                if (U.Length != 1) throw new Exception("Something went really wrong.");

                for (int i = 0; i < solutionLength; i++)
                {
                    if (c[i] == int.MaxValue)
                    {
                        c[i] = U[0];
                        break;
                    }
                }
            }
            
            if (k >= n - 1) // throw new Exception($"Set H is too large, H: {k}, N: {n}"); // n - k - 1 w mianowniku
            {
                SolutionQAP s = new SolutionQAP();
                s.SetSolutionRepresentation(c.ToList());
                return s.GetCost();
            }

            int[] N = Enumerable.Range(0, n).ToArray();
            int[] N_H = N.Except(H).ToArray(); // N\H

            int sum1 = 0, sum2 = 0, sum3 = 0, sum4 = 0, sum5 = 0;

            // sum 1
            foreach (int i in H)
            {
                foreach (int j in H)
                {
                    sum1 += facilityFlows[i, j] * locationDistances[c[i], c[j]];
                }
            }

            // sum 2
            foreach (int i in H)
            {
                foreach (int j in N_H)
                {
                    int s = 0;
                    foreach (int u in U)
                        s += locationDistances[c[i], u];
                    
                    sum2 += facilityFlows[i, j] * s;
                }
            }

            // sum 3
            foreach (int i in N_H)
            {
                foreach (int j in H)
                {
                    int s = 0;
                    foreach (int u in U)
                        s += locationDistances[u, c[j]];
                    
                    sum3 += facilityFlows[i, j] * s;
                }
            }

            // sum 4
            foreach (int i in N_H)
            {
                int s = 0;
                foreach (int u in U)
                    s += locationDistances[u, u];

                sum4 += facilityFlows[i, i] * s;
            }

            // sum 5
            foreach (int i in N_H)
            {
                foreach (int j in N_H)
                {
                    if (i == j) continue;

                    int s = 0;
                    foreach (int o in U)
                    {
                        foreach (int l in U)
                        {
                            if (o == l) continue;
                            s += locationDistances[o, l];
                        }
                    }

                    sum5 += facilityFlows[i, j] * s;
                }
            }

            double m1 = 1.0 / (n - k);
            double m2 = 1.0 / (n - k - 1);

            return (double)sum1 + (m1 * sum2) + (m1 * sum3) + (m1 * sum4) + (m1 * m2 * sum5);
        }


        public void LocalOptimization()
        {
            List<int> repr = GetSolutionRepresentation();
            SetSolutionRepresentation(AlgorithmUtils.TwoOpt.Optimize(repr));
        }
    }
}
