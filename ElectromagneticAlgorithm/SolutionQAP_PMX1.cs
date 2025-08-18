using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectromagneticAlgorithm
{
    public class SolutionQAP_PMX1 : SolutionQAP
    {
        public override ISolution GetCopy()
        {
            SolutionQAP_PMX1 newSolution = new();
            newSolution.SetSolutionRepresentation(GetSolutionRepresentation());
            return newSolution;
        }

        public override void PullTowardsSolution(ISolution secondSolution, double secondSolForce, ISolution[] neighbouringSolutions, double[] attractionForces, int k, bool exploration)
        {
            if (k <= 0) throw new Exception("k must be 1 or greater.");

            SolutionQAP solution2 = AlgorithmUtils.ValidateSolutionType<SolutionQAP>(secondSolution);
            List<int> s1 = new(this.GetSolutionRepresentation());
            List<int> s2 = new(solution2.GetSolutionRepresentation());

            double forceRatio = secondSolForce / attractionForces.Sum();
            int addedRange = (int)Math.Ceiling(forceRatio * solutionLength);

            if (addedRange == 0)
            {
                Console.WriteLine("Warning! Right side of the range must not be equal to the left side.");
                return;
            }

            if (addedRange > maxMatchingRegion)
            {
                addedRange = maxMatchingRegion;
            }
            //Console.WriteLine(addedRange);

            // Establish the mapping region randomly k times and find the best one
            int l = 0, r = 0;
            double bestCost = exploration ? 0.0 : double.MaxValue;
            int bestl = 0;

            if (addedRange <= 1)
            {
                l = random.Next(solutionLength);
                r = (l + addedRange) % solutionLength;
                StandardPMX(secondSolution, l, r);
                return;
            }

            for (int i = 0; i < k; i++)
            {
                l = random.Next(solutionLength); // first index
                r = (l + addedRange) % solutionLength; // last index + 1
                // c1 is the one outside the mapping region, we take the region from c2 and put it in c1
                int[] c1 = new int[solutionLength], c2 = new int[solutionLength];

                // Inside of the region
                int n = l;
                while (n != r) // This will loop around if l > r (which can happen)
                {
                    c2[n] = s2[n];
                    c1[n] = int.MaxValue;

                    n++;
                    n %= solutionLength;
                }

                // Outside of the region
                n = r;
                while (n != l)
                {
                    c1[n] = s1[n];
                    c2[n] = int.MaxValue;

                    n++;
                    n %= solutionLength;
                }

                double newCost = GetConditionalExpectedCost(c1); // + GetConditionalExpectedCost(c2);
                if ((exploration && newCost > bestCost) || (!exploration && newCost < bestCost))
                {
                    bestCost = newCost;
                    bestl = l;
                }

            }
            
            // PMX
            StandardPMX(secondSolution, bestl, (bestl + addedRange) % solutionLength);
        }
    }
}
