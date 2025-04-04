using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectromagneticAlgorithm
{
    public interface ISolution
    {
        public static double GetAverageCost() { return -1; }

        public int GetCost();

        public void CrossoverWithSolution(ISolution solution, int l, int r);

        public void SwapElementsMatchingWithSolution(ISolution solution, int noElements);

        public int GetHammingDistanceFromSolution(ISolution solution);

        public int GetSolutionLength();
        
        public ISolution GetCopy();
    }
}
