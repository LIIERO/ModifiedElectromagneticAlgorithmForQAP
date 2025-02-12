using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ElectromagneticAlgorithm
{
    public class EMSolver
    {
        private int maxIter;
        private int maxLocalIter;
        private List<IPermutationSolution> solutionPopulation;

        private IPermutationSolution bestGlobalSolution;

        public EMSolver(IPermutationSolution[] initialPopulation, int maxIter, int maxLocalIter)
        {
            solutionPopulation = initialPopulation.ToList();
            this.maxIter = maxIter;
            this.maxLocalIter = maxLocalIter;

            bestGlobalSolution = initialPopulation[0];
            UpdateBestGlobalSolution();
        }

        ~EMSolver() { }

        public void StartAlgorithm()
        {
            
        }

        private void UpdateBestGlobalSolution()
        {
            int costThreshold = bestGlobalSolution.GetCost();

            foreach (IPermutationSolution solution in solutionPopulation)
            {
                int solutionCost = solution.GetCost();
                if (solutionCost < costThreshold)
                {
                    bestGlobalSolution = solution;
                    costThreshold = solutionCost;
                }
            }
        }

        public static void AttractionInjection(IPermutationSolution solution, List<IPermutationSolution> neighbouringSolutions)
        {
            // Randomly choose the mapping section
            Random random = new();
            int n = solution.GetSolutionLength();
            int r1 = random.Next(n); int r2 = random.Next(n);
            while (r1 == r2) r2 = random.Next(n);
            (int l, int r) = r2 > r1 ? (r1, r2) : (r2, r1);
            Console.WriteLine($"l = {l}, r = {r}");

            // Determine charge of the point
            int i = 1;
            foreach (IPermutationSolution sol in neighbouringSolutions)
            {
                i += 1;
            }
        }

        public static void PMX(IPermutationSolution solution1, IPermutationSolution solution2, int l, int r)
        {
            // Get copies of solutions
            List<int> s1 = new(solution1.GetSolutionRepresentation());
            List<int> s2 = new(solution2.GetSolutionRepresentation());
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
            solution1.SetSolutionRepresentation(s1);
            solution2.SetSolutionRepresentation(s2);
        }

        public void PrintPopulation()
        {
            Console.WriteLine("Population:");
            foreach(IPermutationSolution solution in solutionPopulation)
            {
                Console.WriteLine(solution.ToString());
            }
        }
    }
}
