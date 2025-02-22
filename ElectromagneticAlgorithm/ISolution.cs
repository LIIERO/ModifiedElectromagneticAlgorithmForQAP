using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectromagneticAlgorithm
{
    public interface ISolution
    {
        static ISolution() { }

        public int GetCost();

        public void CrossoverWithSolution(ISolution solution, int l, int r);

        public int GetHammingDistanceFromSolution(ISolution solution);

        public int GetSolutionLength();

        //public List<T> GetSolutionRepresentation<T>();
    }
}
