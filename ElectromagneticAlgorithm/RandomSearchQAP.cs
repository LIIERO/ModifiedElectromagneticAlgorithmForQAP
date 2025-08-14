using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectromagneticAlgorithm
{
    public class RandomSearchQAP
    {
        private SolutionQAP solution;
        private SolutionQAP bestSolution;

        public RandomSearchQAP(SolutionQAP solution)
        {
            this.solution = solution;
            bestSolution = (SolutionQAP)solution.GetCopy();
        }

        public void Search(long timeMS)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            while (watch.ElapsedMilliseconds < timeMS)
            {
                double solCost = solution.GetCost();
                if (solCost < bestSolution.GetCost())
                {
                    bestSolution = (SolutionQAP)solution.GetCopy();
                    Console.WriteLine($"New best solution: {solCost}");
                }

                List<int> repr = solution.GetSolutionRepresentation();
                AlgorithmUtils.Shuffle(repr);
                solution.SetSolutionRepresentation(repr);
            }
        }

    }
}
