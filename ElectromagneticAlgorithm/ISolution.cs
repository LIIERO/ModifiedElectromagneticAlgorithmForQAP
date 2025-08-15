using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectromagneticAlgorithm
{
    public interface ISolution
    {
        //public static double GetAverageCost() { return -1; }
        public void SetSolutionRepresentation<T>(T repr);
        public void ShuffleRepresentation(int? seed = null);

        public double GetCost();

        public void PullTowardsSolution(ISolution secondSolution, double secondSolForce, ISolution[] neighbouringSolutions, double[] attractionForces, int k, bool exploration); // CrossoverWithSolution

        public void RepelFromSolution(ISolution secondSolution, double secondSolForce, ISolution[] neighbouringSolutions, double[] attractionForces, int k, bool exploration); // SwapElementsMatchingWithSolution

        public double GetDistanceFromSolution(ISolution solution);

        //public static double GetConditionalExpectedCost(int[] c) { return -1; } // c -> partial solution with int.MaxValue in ambiguous spots.

        //public int GetSolutionLength(); // TODO: usuń
        
        public ISolution GetCopy();
    }
}
