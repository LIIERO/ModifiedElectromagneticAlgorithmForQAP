using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectromagneticAlgorithm
{
    public interface ISolution
    {
        public void SetSolutionRepresentation<T>(T repr); // Optional - not used by EM

        public void ShuffleRepresentation(int? seed = null); // Optional - not used by EM

        public ISolution GetCopy();

        public double GetCost();

        public void PullTowardsSolution(ISolution secondSolution, double secondSolForce, ISolution[] neighbouringSolutions, double[] attractionForces, int k, bool exploration); // CrossoverWithSolution

        public void RepelFromSolution(ISolution secondSolution, double secondSolForce, ISolution[] neighbouringSolutions, double[] attractionForces, int k, bool exploration); // SwapElementsMatchingWithSolution

        public double GetDistanceFromSolution(ISolution solution);
    }
}
