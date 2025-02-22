using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace ElectromagneticAlgorithm
{
    public class SolutionQAP : ISolution
    {
        private static int[][] facilityFlows;
        private static int[][] locationDistances;

        private List<int> assignmentPermutation;
        private int solutionLength;

        public static void SetQAPData(int[][] flows, int[][] distances)
        {
            facilityFlows = flows;
            locationDistances = distances;
        }
        
        static SolutionQAP()
        {
            // TODO: Wczytywanie danych facility flows, location distances z pliku
        }

        public SolutionQAP()
        {
            if (facilityFlows == null || locationDistances == null) throw new AlgorithmUtils.SolutionNotInitializedException();

            solutionLength = facilityFlows.Length;
            assignmentPermutation = Enumerable.Range(0, solutionLength).ToList();
        }

        public int GetCost()
        {
            
            int totalCost = 0;
            int n = facilityFlows.Length;

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    int facility1 = assignmentPermutation[i];
                    int facility2 = assignmentPermutation[j];
                    int location1 = i;
                    int location2 = j;

                    totalCost += facilityFlows[facility1][facility2] * locationDistances[location1][location2];
                }
            }

            return totalCost;
        }

        public void CrossoverWithSolution(ISolution secondSolution, int l, int r)
        {
            SolutionQAP solution2 = AlgorithmUtils.ValidateSolutionType<SolutionQAP>(secondSolution);

            // Get copies of solutions
            List<int> s1 = new(this.GetSolutionRepresentation());
            List<int> s2 = new(solution2.GetSolutionRepresentation());
            AlgorithmUtils.ValidatePermutation(s2, solutionLength);

            List<int> s1Copy = new(s1);
            int range = r - l;
            if (range <= 0) throw new Exception("Right side of the range needs to be higher than the left side.");

            // Swap a slice of s1 with slice of s2, and vice versa + Determine the mapping relationship
            AlgorithmUtils.Map<int, int> mappingRelationship = new();
            for (int i = l; i < r; i++)
            {
                s1[i] = s2[i];
                s2[i] = s1Copy[i];
                mappingRelationship.Add(s1[i], s2[i]);
            }

            // Legalize solution with the mapping relationship
            List<int> forwardElements = mappingRelationship.Forward.GetKeys();
            List<int> reverseElements = mappingRelationship.Reverse.GetKeys();

            void LegalizeSolution(int i)
            {
                if (forwardElements.Contains(s1[i]))
                    s1[i] = mappingRelationship.Forward[s1[i]];
                if (reverseElements.Contains(s2[i]))
                    s2[i] = mappingRelationship.Reverse[s2[i]];
            }

            for (int i = 0; i < l; i++) // Elements before the slice
                LegalizeSolution(i);
            for (int i = r; i < s1.Count; i++) // Elements after the slice
                LegalizeSolution(i);

            // Replace the solutions
            this.SetSolutionRepresentation(s1);
            solution2.SetSolutionRepresentation(s2);
        }

        public int GetHammingDistanceFromSolution(ISolution solution)
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

        public int GetSolutionLength()
        {
            return solutionLength;
        }

        public List<int> GetSolutionRepresentation()
        {
            return assignmentPermutation;
        }

        public void SetSolutionRepresentation(List<int> repr)
        {
            AlgorithmUtils.ValidatePermutation(repr, solutionLength);

            assignmentPermutation = repr;
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
    }
}
