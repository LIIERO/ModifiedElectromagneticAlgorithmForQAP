using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace ElectromagneticAlgorithm
{
    public class SolutionQAP : IPermutationSolution
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

        public SolutionQAP()
        {
            if (facilityFlows == null || locationDistances == null) throw new Exception(); // TODO: Custom exceptions

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
