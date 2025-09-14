using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace ElectromagneticAlgorithm
{
    public class SolutionQAP_PMX2 : SolutionQAP
    {
        public override ISolution GetCopy()
        {
            SolutionQAP_PMX2 newSolution = new();
            newSolution.SetSolutionRepresentation(GetSolutionRepresentation());
            return newSolution;
        }

        public override void PullTowardsSolution(ISolution secondSolution, double secondSolForce, ISolution[] neighbouringSolutions, double[] attractionForces, int k, bool exploration)
        {
            int l = random.Next(solutionLength);
            double forceRatio = secondSolForce / attractionForces.Sum();
            int addedRange = (int)Math.Ceiling(forceRatio * solutionLength);

            if (addedRange > maxMatchingRegion)
            {
                addedRange = maxMatchingRegion;
            }

            int r = (l + addedRange) % solutionLength;

            // PMX
            SolutionQAP solution2 = AlgorithmUtils.ValidateSolutionType<SolutionQAP>(secondSolution);

            List<int> s1 = new(this.GetSolutionRepresentation());
            List<int> s2 = new(solution2.GetSolutionRepresentation());
            AlgorithmUtils.ValidatePermutation(s2, solutionLength);

            List<int> s1Copy = new(s1);
            int range = r - l;

            if (range == 0)
            {
                Console.WriteLine("Warning! Right side of the range must not be equal to the left side.");
                return;
            }

            AlgorithmUtils.Map<int, int> mappingRelationship = new();
            int n = l;
            while (n != r)
            {
                s1[n] = s2[n];
                s2[n] = s1Copy[n];
                mappingRelationship.Add(s1[n], s2[n]);

                n++;
                n %= solutionLength;
            }

            // Legalizacja rozwiązania
            List<int> elementsToAdd = new();
            for (int i = 0; i < solutionLength; i++)
            {
                if (!mappingRelationship.Forward.GetKeys().Contains(i))
                    elementsToAdd.Add(i);
            }
            int[] c = Enumerable.Repeat(int.MaxValue, solutionLength).ToArray();
            n = l;
            while (n != r)
            {
                c[n] = s1[n];
                n++;
                n %= solutionLength;
            }
            int[] cBest = new int[solutionLength];
            int[] cCopy = new int[solutionLength];

            while (elementsToAdd.Count > 1)
            {
                double bestCost = exploration ? 0.0 : double.MaxValue;
                List<int> elementsToAddBest = new(elementsToAdd);
                Array.Copy(c, cBest, solutionLength);

                for (int j = 0; j < k; j++)
                {
                    List<int> elementsToAddCopy = new(elementsToAdd);
                    Array.Copy(c, cCopy, solutionLength);

                    int randomElement = elementsToAddCopy[random.Next(elementsToAddCopy.Count)];
                    int emptySpotToAdd = random.Next(elementsToAddCopy.Count);
                    int currentEmptySpot = 0;
                    for (int i = 0; i < solutionLength; i++)
                    {
                        if (cCopy[i] == int.MaxValue)
                        {
                            if (currentEmptySpot == emptySpotToAdd)
                            {
                                cCopy[i] = randomElement;
                                elementsToAddCopy.Remove(randomElement);
                                break;
                            }
                            currentEmptySpot += 1;
                        }
                    }

                    double newCost = GetConditionalExpectedCost(cCopy); // + GetConditionalExpectedCost(c2);

                    if ((exploration && newCost > bestCost) || (!exploration && newCost < bestCost))
                    {
                        bestCost = newCost;
                        elementsToAddBest = new(elementsToAddCopy);
                        Array.Copy(cCopy, cBest, solutionLength);
                    }
                }

                elementsToAdd = new(elementsToAddBest);
                Array.Copy(cBest, c, solutionLength);
            }

            for (int i = 0; i < solutionLength; i++)
            {
                if (c[i] == int.MaxValue)
                {
                    c[i] = elementsToAdd[0];
                    elementsToAdd.RemoveAt(0);
                }
            }

            this.SetSolutionRepresentation(c.ToList());
        }
    }
}
