using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectromagneticAlgorithm
{
    public interface IPermutationSolution
    {
        public int GetCost();

        public List<int> GetSolutionRepresentation();
        public int GetSolutionLength();

        public void SetSolutionRepresentation(List<int> solutionRepresentation);

        //public List<int> Representation { get; set; }
    }
}
