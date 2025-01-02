namespace ElectromagneticAlgorithm
{
    public class EMSolver
    {
        private int maxIter;
        private int maxLocalIter;
        private List<IPermutationSolution> solutionPopulation;

        private IPermutationSolution bestGlobalSolution;

        public EMSolver(List<IPermutationSolution> initialPopulation, int maxIter, int maxLocalIter)
        {
            solutionPopulation = initialPopulation;
            this.maxIter = maxIter;
            this.maxLocalIter = maxLocalIter;
        }

        ~EMSolver() { }

        public void StartAlgorithm()
        {
            
        }
    }
}
