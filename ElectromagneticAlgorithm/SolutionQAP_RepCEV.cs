using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace ElectromagneticAlgorithm
{
    public class SolutionQAP_RepCEV : SolutionQAP
    {
        public override ISolution GetCopy()
        {
            SolutionQAP_RepCEV newSolution = new();
            newSolution.SetSolutionRepresentation(GetSolutionRepresentation());
            return newSolution;
        }

        public override void RepelFromSolution(ISolution secondSolution, double secondSolForce, ISolution[] neighbouringSolutions, double[] attractionForces, int k, bool exploration)
        {
            int sameElementsSum = 0;
            for (int j = 0; j < neighbouringSolutions.Length; j++)
                sameElementsSum += solutionLength - (int)GetDistanceFromSolution(neighbouringSolutions[j]);

            int noElementsInSamePositions = solutionLength - (int)GetDistanceFromSolution(secondSolution);
            double forceRatio = secondSolForce / attractionForces.Sum();

            int noElements = Math.Min((int)Math.Ceiling(sameElementsSum * forceRatio), noElementsInSamePositions); // Wzór (4) praca WCH

            if (noElements <= 1) return;

            SolutionQAP solution2 = AlgorithmUtils.ValidateSolutionType<SolutionQAP>(secondSolution);

            List<int> s1 = new(this.GetSolutionRepresentation());
            List<int> s2 = new(solution2.GetSolutionRepresentation());
            AlgorithmUtils.ValidatePermutation(s2, solutionLength);

            List<int> matchingElementsIndexes = new();
            for (int i = 0; i < solutionLength; i++)
            {
                if (s1[i] == s2[i])
                {
                    matchingElementsIndexes.Add(i);
                }
            }

            List<int> matchingElementsIndexesCopy = new List<int>(matchingElementsIndexes);
            bool badShuffle = true;
            while (badShuffle)
            {
                badShuffle = false;
                AlgorithmUtils.Shuffle(matchingElementsIndexes);
                for (int i = 0; i < matchingElementsIndexes.Count; i++)
                {
                    if (matchingElementsIndexes[i] == matchingElementsIndexesCopy[i])
                    {
                        badShuffle = true;
                        matchingElementsIndexes = new List<int>(matchingElementsIndexesCopy);
                        break;
                    }
                }
            }

            List<int> bestIndexesToShuffle = new();
            double bestCost = exploration ? 0.0 : double.MaxValue;
            for (int i = 0; i < k; i++)
            {
                List<int> indexesToShuffleUnsorted = new(matchingElementsIndexes);
                while (indexesToShuffleUnsorted.Count > noElements)
                {
                    indexesToShuffleUnsorted.RemoveAt(random.Next(indexesToShuffleUnsorted.Count));
                }

                int[] c = new int[solutionLength];
                for (int j = 0; j < solutionLength; j++)
                {
                    if (!indexesToShuffleUnsorted.Contains(j))
                        c[j] = int.MaxValue;
                    else
                        c[j] = s1[j];
                }

                double newCost = GetConditionalExpectedCost(c);

                if ((exploration && newCost > bestCost) || (!exploration && newCost < bestCost))
                {
                    bestCost = newCost;
                    bestIndexesToShuffle = new(indexesToShuffleUnsorted);
                }
            }

            List<int> indexesToShuffle = new List<int>(bestIndexesToShuffle);
            indexesToShuffle.Sort();


            List<int> s1copy = new List<int>(s1);
            for (int i = 0; i < indexesToShuffle.Count; i++)
            {
                s1[indexesToShuffle[i]] = s1copy[bestIndexesToShuffle[i]];
            }

            this.SetSolutionRepresentation(s1);
        }
    }
}
