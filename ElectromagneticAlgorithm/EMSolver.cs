using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ElectromagneticAlgorithm
{
    public class EMSolver
    {
        private int maxIter;
        private int maxLocalIter;
        private List<ISolution> solutionPopulation;

        private ISolution bestGlobalSolutionEver;

        public EMSolver(ISolution[] initialPopulation, int maxIter, int maxLocalIter)
        {
            solutionPopulation = initialPopulation.ToList();
            this.maxIter = maxIter;
            this.maxLocalIter = maxLocalIter;

            bestGlobalSolutionEver = initialPopulation[0];
            UpdateBestGlobalSolution();
        }

        ~EMSolver() { }


        public void StartAlgorithm()
        {
            
        }

        private ISolution GetBestSolutionFromPopulation()
        {
            return solutionPopulation.MinBy(sol => sol.GetCost());
        }

        private double GetSolutionCharge(ISolution solution)
        {
            int bestSolutionCost = GetBestSolutionFromPopulation().GetCost();
            return Math.Exp(-1 * ((solution.GetCost() - bestSolutionCost) / bestSolutionCost));
        }

        private ISolution[] GetNeighbouringSolutions(ISolution solution, int maxNeighbouringDistance)
        {
            List<ISolution> neighbouringSolutions = new();

            foreach (ISolution sol in solutionPopulation)
            {
                if (solution != sol && solution.GetHammingDistanceFromSolution(sol) <= maxNeighbouringDistance)
                {
                    neighbouringSolutions.Add(sol);
                }
            }
            return neighbouringSolutions.ToArray();
        }

        private void UpdateBestGlobalSolution() // TODO: Simplify like in GetBestSolutionFromPopulation
        {
            int costThreshold = bestGlobalSolutionEver.GetCost();

            foreach (ISolution solution in solutionPopulation)
            {
                int solutionCost = solution.GetCost();
                if (solutionCost < costThreshold)
                {
                    bestGlobalSolutionEver = solution;
                    costThreshold = solutionCost;
                }
            }
        }

        public void AttractionInjection(ISolution solution, ISolution[] neighbouringSolutions)
        {
            // Randomly choose the mapping section
            Random random = new();
            int n = solution.GetSolutionLength();
            int r1 = random.Next(n); int r2 = random.Next(n);
            while (r1 == r2) r2 = random.Next(n);
            (int l, int r) = r2 > r1 ? (r1, r2) : (r2, r1);
            Console.WriteLine($"l = {l}, r = {r}");

            // Determine attraction forces for each neighbouring solution
            int d = neighbouringSolutions.Length;
            double[] attractionForces = new double[d];

            for (int i = 0; i < d; i++)
            {
                double solCharge = GetSolutionCharge(neighbouringSolutions[i]);
                attractionForces[i] = solCharge / Math.Pow(solution.GetHammingDistanceFromSolution(neighbouringSolutions[i]), 2);
            }
                

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
