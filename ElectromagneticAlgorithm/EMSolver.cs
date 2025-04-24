using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using static System.Collections.Specialized.BitVector32;

namespace ElectromagneticAlgorithm
{
    public class EMSolver
    {
        private int maxIter;
        private int activeSolutionSampleSize;
        private int neighbourhoodDistance;
        private double attractionProbability; // Greater than 0, less than 1
        private ISolution[] solutionPopulation;
        private ISolution[] solutionPopulationSubset;

        private ISolution bestGlobalSolutionEver;
        private Random random;

        private StringBuilder bestSolutionCSV;
        private bool saveBestSolutionData = false;
        private string bestSolutionDataPath;

        public EMSolver(ISolution[] initialPopulation, int maxIter, int activeSolutionSampleSize, int neighbourhoodDistance, double attractionProbability)
        {
            solutionPopulation = initialPopulation;
            this.maxIter = maxIter;
            this.activeSolutionSampleSize = activeSolutionSampleSize;
            this.neighbourhoodDistance = neighbourhoodDistance;
            this.attractionProbability = attractionProbability;
            random = new Random();

            //bestGlobalSolutionEver = initialPopulation[0];
            //UpdateBestGlobalSolution();
            bestGlobalSolutionEver = GetBestSolutionFromPopulation().GetCopy();
            Console.WriteLine($"Best initial solution cost: {bestGlobalSolutionEver.GetCost()}");
        }

        ~EMSolver() { }


        public void InitializeBestSolutionDataSaver(string absolutePath)
        {
            saveBestSolutionData = true;
            bestSolutionDataPath = absolutePath;

            bestSolutionCSV = new StringBuilder();
            bestSolutionCSV.AppendLine("Iteration,Best local solution cost,Best global solution cost");
        }


        public void RunAlgorithm(float subsetRatio=0.5f)
        {
            // Global iteration
            for (int i = 0; i < maxIter; i++)
            {
                Console.WriteLine($"\nIteration {i}");

                // Select a random population subset
                int populationLength = solutionPopulation.Length;
                int populationSubsetSize = (int)(populationLength * subsetRatio);
                solutionPopulationSubset = AlgorithmUtils.ChooseRandom(solutionPopulation, populationSubsetSize);

                bool isExploring = false;
                if (random.NextDouble() < Math.Exp(-i / CalculateEntropy(solutionPopulation)))
                    isExploring = true;

                bool isAttracting = false;
                if (random.NextDouble() < attractionProbability)
                    isAttracting = true;

                isExploring = !isExploring;
                Console.WriteLine($"is attracting: {isAttracting}, is exploring: {isExploring}");

                bool betterObjectiveValueNeighbours = (isExploring && !isAttracting) || (!isExploring && isAttracting);

                foreach (ISolution solution in solutionPopulationSubset)
                {
                    ISolution[] neighbouringSubset = ChooseSolutionsInHammingDistance(solution, betterObjectiveValueNeighbours);

                    if (isAttracting)
                        AttractionInjection(solution, neighbouringSubset);
                    else
                        RepulsionSwap(solution, neighbouringSubset);

                    //AttractionInjection(solution, neighbouringSubset);
                }

                ISolution bestLocalSolution = GetBestSolutionFromPopulation();
                int bestLocalSolutionCost = bestLocalSolution.GetCost();

                if (bestLocalSolutionCost < bestGlobalSolutionEver.GetCost()) bestGlobalSolutionEver = bestLocalSolution.GetCopy();

                if (saveBestSolutionData)
                    bestSolutionCSV.AppendLine($"{i},{bestLocalSolutionCost},{bestGlobalSolutionEver.GetCost()}");
            }

            FinishAlgorithm();
        }

        private void FinishAlgorithm()
        {
            Console.WriteLine($"\nBest final solution cost: {bestGlobalSolutionEver.GetCost()}");

            if (saveBestSolutionData)
            {
                File.WriteAllText(bestSolutionDataPath, bestSolutionCSV.ToString());
                Console.WriteLine($"Saved best solution data to {bestSolutionDataPath}");
                //saveBestSolutionData = false;
            }
        }

        private ISolution GetBestSolutionFromPopulation()
        {
            return solutionPopulation.MinBy(sol => sol.GetCost());
        }

        private double GetSolutionCharge(ISolution solution)
        {
            int bestSolutionCost = GetBestSolutionFromPopulation().GetCost();
            return Math.Exp(-1 * ((solution.GetCost() - bestSolutionCost) / bestSolutionCost));
        }

        private ISolution[] ChooseSolutionsInHammingDistance(ISolution solution, bool betterObjectiveValue)
        {
            int solutionCost = solution.GetCost();
            ISolution[] neighbouringSolutions = GetNeighbouringSolutions(solution, solutionPopulationSubset, neighbourhoodDistance);
            List<ISolution> solutions = new();
            foreach (ISolution neighbour in neighbouringSolutions)
            {
                if (betterObjectiveValue)
                {
                    if (neighbour.GetCost() > solutionCost)
                        solutions.Add(neighbour);
                }
                else
                {
                    if (neighbour.GetCost() < solutionCost)
                        solutions.Add(neighbour);
                }
            }
            // TODO: randomly choosing k solutions with probability
            return solutions.ToArray();
        }

        private double CalculateEntropy(ISolution[] population)
        {
            int d = population.Length;
            double entropy = 0;

            for (int i = 0; i < d; i++)
            {
                double proportion = population[i].GetCost() / population.Sum(sol => sol.GetCost());
                entropy += proportion * Math.Log2(proportion);
            }

            return -entropy;
        }

        private static ISolution[] GetNeighbouringSolutions(ISolution solution, ISolution[] population, int maxNeighbouringDistance)
        {
            List<ISolution> neighbouringSolutions = new();

            foreach (ISolution sol in population)
            {
                if (solution != sol && solution.GetHammingDistanceFromSolution(sol) <= maxNeighbouringDistance)
                {
                    neighbouringSolutions.Add(sol);
                }
            }
            return neighbouringSolutions.ToArray();
        }

        /*private void UpdateBestGlobalSolution() // TODO: Simplify like in GetBestSolutionFromPopulation
        {
            int costThreshold = bestGlobalSolutionEver.GetCost();

            foreach (ISolution solution in solutionPopulation)
            {
                int solutionCost = solution.GetCost();
                if (solutionCost < costThreshold)
                {
                    bestGlobalSolutionEver = solution;
                    costThreshold = solutionCost;
                }
            }
        }*/

        public ISolution AttractionInjection(ISolution solution, ISolution[] neighbouringSolutions)
        {
            // Randomly choose the mapping section
            Random random = new();
            int solLen = solution.GetSolutionLength();
            int r1 = random.Next(solLen); int r2 = random.Next(solLen);
            while (r1 == r2) r2 = random.Next(solLen);
            (int l, int r) = r2 > r1 ? (r1, r2) : (r2, r1);
            //Console.WriteLine($"l = {l}, r = {r}");

            // Determine attraction forces for each neighbouring solution
            int d = neighbouringSolutions.Length;
            //Console.WriteLine(d);
            double[] attractionForces = new double[d];

            for (int i = 0; i < d; i++)
            {
                double solCharge = GetSolutionCharge(neighbouringSolutions[i]);
                attractionForces[i] = solCharge / Math.Pow(solution.GetHammingDistanceFromSolution(neighbouringSolutions[i]), 2);
            }

            // Insert a part of each solution to the mapping section of the given solution proportionally to the power of attraction
            double attractionForcesSum = attractionForces.Sum();
            int lP = l;

            for (int i = 0; i < d; i++)
            {
                double forceRatio = attractionForces[i] / attractionForcesSum;
                //Console.WriteLine(forceRatio);
                int addedRange = (int)Math.Ceiling(forceRatio * solLen);
                //Console.WriteLine(addedRange);
                if (addedRange == 0) addedRange = 1;
                if (addedRange == solLen) addedRange = solLen - 1;

                int rP = lP + addedRange;
                rP %= solLen;

                solution.CrossoverWithSolution(neighbouringSolutions[i], lP, rP);
                lP = rP + 1;
                lP %= solLen;
            }

            return solution;
        }

        public ISolution RepulsionSwap(ISolution solution, ISolution[] neighbouringSolutions)
        {
            // Determine attraction forces for each neighbouring solution
            int d = neighbouringSolutions.Length;
            double[] attractionForces = new double[d];
            int solLen = solution.GetSolutionLength();

            for (int i = 0; i < d; i++)
            {
                double solCharge = GetSolutionCharge(neighbouringSolutions[i]);
                attractionForces[i] = solCharge / Math.Pow(solution.GetHammingDistanceFromSolution(neighbouringSolutions[i]), 2);
            }

            double attractionForcesSum = attractionForces.Sum();
            int sameElementsSum = 0;
            for (int j = 0; j < d; j++)
                sameElementsSum += solLen - solution.GetHammingDistanceFromSolution(neighbouringSolutions[j]);
            
            for (int i = 0; i < d; i++)
            {
                int noElementsInSamePositions = solLen - solution.GetHammingDistanceFromSolution(neighbouringSolutions[i]);
                double forceRatio = attractionForces[i] / attractionForcesSum;
                
                int noToSwap = Math.Min((int)Math.Ceiling(sameElementsSum * forceRatio), noElementsInSamePositions); // Wzór (4) praca WCH

                solution.SwapElementsMatchingWithSolution(neighbouringSolutions[i], noToSwap);
            }

            return solution;
        }

        public void PrintPopulation()
        {
            Console.WriteLine("\nPopulation:");
            foreach(ISolution solution in solutionPopulation) // TODO: działa z QAP solution, znajdź czemu nie działa z interfejsem
            {
                Console.WriteLine(solution.ToString());
                Console.WriteLine(solution.GetCost());
            }
        }
    }
}
