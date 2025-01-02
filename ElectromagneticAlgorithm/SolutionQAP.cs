using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectromagneticAlgorithm
{
    public class SolutionQAP : IPermutationSolution
    {
        private static int[][] facilityFlows;
        private static int[][] locationDistances;

        private List<int> assignmentPermutation;

        public static void SetQAPData(int[][] flows, int[][] distances)
        {
            facilityFlows = flows;
            locationDistances = distances;
        }

        public SolutionQAP()
        {
            if (facilityFlows == null || locationDistances == null) throw new Exception(); // TODO: Custom exceptions

            int n = facilityFlows.Length;
            assignmentPermutation = Enumerable.Range(0, n).ToList();
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

        public List<int> GetSolutionRepresentation()
        {
            return assignmentPermutation;
        }
    }
}
