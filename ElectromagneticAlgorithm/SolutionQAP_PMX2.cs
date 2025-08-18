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
            // Establish the mapping region randomly
            int l = random.Next(solutionLength);
            double forceRatio = secondSolForce / attractionForces.Sum();
            int addedRange = (int)Math.Ceiling(forceRatio * solutionLength);

            if (addedRange > maxMatchingRegion)
            {
                addedRange = maxMatchingRegion;
            }
            //Console.WriteLine(addedRange);
            int r = (l + addedRange) % solutionLength;

            //Console.WriteLine($"l = {l}, r = {r}");

            // PMX
            SolutionQAP solution2 = AlgorithmUtils.ValidateSolutionType<SolutionQAP>(secondSolution);

            // Get copies of solutions
            List<int> s1 = new(this.GetSolutionRepresentation());
            List<int> s2 = new(solution2.GetSolutionRepresentation());
            AlgorithmUtils.ValidatePermutation(s2, solutionLength);

            List<int> s1Copy = new(s1);
            int range = r - l;
            //if (range <= 0) throw new Exception("Right side of the range needs to be higher than the left side.");
            //if (range == 0) throw new Exception("Right side of the range must not be equal to the left side.");

            if (range == 0)
            {
                Console.WriteLine("Warning! Right side of the range must not be equal to the left side.");
                return;
            }

            // Swap a slice of s1 with slice of s2
            AlgorithmUtils.Map<int, int> mappingRelationship = new();
            int n = l;
            while (n != r) // This will loop around if l > r (which can happen)
            {
                s1[n] = s2[n];
                s2[n] = s1Copy[n];
                mappingRelationship.Add(s1[n], s2[n]);

                n++;
                n %= solutionLength;
            }

            // initialize set of loose elements and incomplete solution
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

                    // Put random element at random empty spot
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

                    // Check if the new c is better
                    double newCost = GetConditionalExpectedCost(cCopy); // + GetConditionalExpectedCost(c2);

                    //Console.WriteLine($"Config found: {randomElement}      {String.Join("; ", elementsToAddCopy)}, cost = {newCost}");
                    //Console.WriteLine($"Config found: {String.Join("; ", cCopy)}");

                    if ((exploration && newCost > bestCost) || (!exploration && newCost < bestCost))
                    {
                        bestCost = newCost;
                        elementsToAddBest = new(elementsToAddCopy);
                        Array.Copy(cCopy, cBest, solutionLength);
                    }
                }

                // Replace the current c with the best found
                elementsToAdd = new(elementsToAddBest);
                Array.Copy(cBest, c, solutionLength);

                //Console.WriteLine($"\nChosen one: {String.Join("; ", elementsToAdd)}, cost = {bestCost}");
                //Console.WriteLine($"Chosen one: {String.Join("; ", c)}");
            }

            // Put the last elements in
            for (int i = 0; i < solutionLength; i++)
            {
                if (c[i] == int.MaxValue)
                {
                    c[i] = elementsToAdd[0];
                    elementsToAdd.RemoveAt(0);
                }
            }

            // Replace the solution
            this.SetSolutionRepresentation(c.ToList());
        }
    }
}
