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
        private int cycleIter;
        private int bonusExploatationIter;
        private int neighbourhoodDistance;
        private int maxNeighbourhoodSize;
        private double attractionProbability; // Greater than 0, less than 1
        private float subsetRatio;
        private int k; // How many tries CEV crossovers have
        double entropyMin;
        double entropyMax;
        private ISolution[] solutionPopulation;
        private ISolution[] solutionPopulationSubset;

        private ISolution bestGlobalSolutionEver;
        private Random random;

        private StringBuilder bestSolutionCSV;
        private bool saveBestSolutionData = false;
        private string bestSolutionDataPath;

        public EMSolver(ISolution[] initialPopulation, int maxIter, int cycleIter, int bonusExploatationIter, int neighbourhoodDistance, int maxNeighbourhoodSize, double attractionProbability, float subsetRatio, int k, double entropyMin, double entropyMax)
        {
            solutionPopulation = initialPopulation;
            this.maxIter = maxIter;
            this.cycleIter = cycleIter;
            this.bonusExploatationIter = bonusExploatationIter;
            this.neighbourhoodDistance = neighbourhoodDistance;
            this.maxNeighbourhoodSize = maxNeighbourhoodSize;
            this.attractionProbability = attractionProbability;
            this.subsetRatio = subsetRatio;
            this.entropyMin = entropyMin;
            this.entropyMax = entropyMax;
            this.k = k;
            random = new Random();

            Console.WriteLine("\n\nModified EM algorithm.");
            Console.WriteLine($"P_init_size = {initialPopulation.Length}, I_max = {maxIter}, d_max = {neighbourhoodDistance}, s_max = {maxNeighbourhoodSize}, p_A = {attractionProbability}, k = {k}");

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
            bestSolutionCSV.AppendLine("Iteration,Best local solution cost,Best global solution cost"); // TODO: Więcej parametrów
        }


        public (double bestSolution, long timeMs) RunAlgorithm()
        {
            double minEntropy = double.MaxValue;
            double maxEntropy = 0;

            int trueMaxIter = maxIter + bonusExploatationIter;

            var watch = System.Diagnostics.Stopwatch.StartNew();

            // Global iteration
            for (int i = 0; i < trueMaxIter; i++)
            {
                Console.WriteLine($"\nIteration {i}");

                // Select a random population subset
                int populationLength = solutionPopulation.Length;
                int populationSubsetSize = (int)(populationLength * subsetRatio);
                solutionPopulationSubset = AlgorithmUtils.ChooseRandom(solutionPopulation, populationSubsetSize);

                double popEntropy = CalculatePopulationEntropy(solutionPopulation);

                if (popEntropy < minEntropy) minEntropy = popEntropy;
                if (popEntropy > maxEntropy) maxEntropy = popEntropy;

                bool isExploring = false;
                if (i < maxIter)
                {
                    Console.WriteLine($"Population entropy: {popEntropy}");
                    double normalizedEntropy = CalculateNormalizedPopulationEntropy(popEntropy, entropyMin, entropyMax, 0.0001);
                    Console.WriteLine($"Normalized entropy: {normalizedEntropy}");
                    //double entropy = Math.Exp(-i / normalizedEntropy);
                    double explorationDecay = 1.0 - (i % cycleIter / (double)cycleIter);
                    Console.WriteLine($"Exploration decay: {explorationDecay}");
                    double entropyChance = explorationDecay * (1.0 - normalizedEntropy); // Im większa entropia tym mniejsza szansa na eksplorację
                    Console.WriteLine($"Calculated chance: {entropyChance}");

                    if (random.NextDouble() < entropyChance)
                        isExploring = true;
                }

                // temporary
                //bool isExploring = true;
                //if (random.NextDouble() < attractionProbability)
                //    isExploring = false;

                // temporary
                //bool isExploring = false;

                bool isAttracting = false;
                if (random.NextDouble() < attractionProbability)
                    isAttracting = true;

                Console.WriteLine($"is attracting: {isAttracting}, is exploring: {isExploring}");

                bool betterObjectiveValueNeighbours = (isExploring && !isAttracting) || (!isExploring && isAttracting);

                /*foreach (ISolution solution in solutionPopulationSubset)
                {
                    ISolution[] neighbouringSubset = ChooseSolutionsInHammingDistance(solution, betterObjectiveValueNeighbours);

                    if (isAttracting)
                        Attraction(solution, neighbouringSubset, isExploring);
                    else
                        Repulsion(solution, neighbouringSubset, isExploring);

                    //AttractionInjection(solution, neighbouringSubset);
                }*/

                // Kopiowanie aktywnej populacji w celu uniknięcia nieścisłości w wykonywaniu ruchów
                ISolution[] popSubsetCopy = new ISolution[populationSubsetSize];
                for (int idx = 0; idx < populationSubsetSize; idx++)
                    popSubsetCopy[idx] = solutionPopulationSubset[idx].GetCopy();
                

                Parallel.ForEach(solutionPopulationSubset, solution =>
                {
                    ISolution[] neighbouringSubset = ChooseSolutionsInHammingDistance(solution, betterObjectiveValueNeighbours, popSubsetCopy);

                    if (isAttracting)
                        Attraction(solution, neighbouringSubset, isExploring);
                    else
                        Repulsion(solution, neighbouringSubset, isExploring);
                });

                ISolution bestLocalSolution = GetBestSolutionFromPopulation();
                double bestLocalSolutionCost = bestLocalSolution.GetCost();

                if (bestLocalSolutionCost < bestGlobalSolutionEver.GetCost())
                {
                    bestGlobalSolutionEver = bestLocalSolution.GetCopy();
                    Console.WriteLine($"New best solution: {bestGlobalSolutionEver.GetCost()}");
                }

                if (saveBestSolutionData)
                    bestSolutionCSV.AppendLine($"{i},{bestLocalSolutionCost},{bestGlobalSolutionEver.GetCost()}");
            }

            watch.Stop();
            long elapsedMs = watch.ElapsedMilliseconds;
            
            //Console.WriteLine($"Min entropy: {minEntropy}");
            //Console.WriteLine($"Max entropy: {maxEntropy}");

            FinishAlgorithm();

            return (bestGlobalSolutionEver.GetCost(), elapsedMs);
        }

        private void FinishAlgorithm()
        {
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
            double bestSolutionCost = GetBestSolutionFromPopulation().GetCost();
            return Math.Exp(-1 * ((solution.GetCost() - bestSolutionCost) / bestSolutionCost));
        }

        private ISolution[] ChooseSolutionsInHammingDistance(ISolution solution, bool betterObjectiveValue, ISolution[] population)
        {
            double solutionCost = solution.GetCost();
            ISolution[] neighbouringSolutions = GetNeighbouringSolutions(solution, population, neighbourhoodDistance);
            List<ISolution> solutions = new();
            foreach (ISolution neighbour in neighbouringSolutions)
            {
                if (betterObjectiveValue) // Better means smaller
                {
                    if (neighbour.GetCost() < solutionCost)
                        solutions.Add(neighbour);
                }
                else
                {
                    if (neighbour.GetCost() > solutionCost)
                        solutions.Add(neighbour);
                }
            }

            if (solutions.Count <= maxNeighbourhoodSize) return solutions.ToArray();

            //Console.Write("Trim");
            //solutions.Shuffle();
            if (betterObjectiveValue) // Take n best or worst solutions
                solutions.OrderBy(sol => sol.GetCost());
            else
                solutions.OrderBy(sol => -sol.GetCost());

            List<ISolution> solutionsTrimmed = solutions.Take(maxNeighbourhoodSize).ToList();
            
            return solutionsTrimmed.ToArray();
        }

        private double CalculateNormalizedPopulationEntropy(double popEntropy, double entropyMin, double entropyMax, double smallValue)
        {
            return (popEntropy - entropyMin) / (entropyMax - entropyMin + smallValue);
        }

        private double CalculatePopulationEntropy(ISolution[] population)
        {
            int d = population.Length;
            double entropy = 0;

            for (int i = 0; i < d; i++)
            {
                double proportion = ((double)population[i].GetCost()) / population.Sum(sol => sol.GetCost());
                entropy += proportion * Math.Log2(proportion);
            }

            //Console.WriteLine(-entropy);
            return -entropy;
        }

        private static ISolution[] GetNeighbouringSolutions(ISolution solution, ISolution[] population, int maxNeighbouringDistance)
        {
            List<ISolution> neighbouringSolutions = new();

            foreach (ISolution sol in population)
            {
                if (solution != sol && solution.GetDistanceFromSolution(sol) <= maxNeighbouringDistance)
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

        public ISolution Attraction(ISolution solution, ISolution[] neighbouringSolutions, bool exploration)
        {
            // Randomly choose the mapping section
            //Random random = new();
            //int solLen = solution.GetSolutionLength();
            //int r1 = random.Next(solLen); int r2 = random.Next(solLen);
            //while (r1 == r2) r2 = random.Next(solLen);
            //(int l, int r) = r2 > r1 ? (r1, r2) : (r2, r1);
            //Console.WriteLine($"l = {l}, r = {r}");

            // Determine attraction forces for each neighbouring solution
            int d = neighbouringSolutions.Length;
            //Console.WriteLine(d);
            double[] attractionForces = new double[d];
            double solutionCharge = GetSolutionCharge(solution);

            for (int i = 0; i < d; i++)
            {
                double charge = GetSolutionCharge(neighbouringSolutions[i]);
                attractionForces[i] = (solutionCharge * charge) / Math.Pow(solution.GetDistanceFromSolution(neighbouringSolutions[i]), 2);
            }

            // Insert a part of each solution to the mapping section of the given solution proportionally to the power of attraction
            double attractionForcesSum = attractionForces.Sum();
            //int lP = l;

            for (int i = 0; i < d; i++)
            {
                //double forceRatio = attractionForces[i] / attractionForcesSum;
                //Console.WriteLine(forceRatio);
                //int addedRange = (int)Math.Ceiling(forceRatio * solLen);
                //Console.WriteLine(addedRange);
                //if (addedRange == 0) addedRange = 1;
                //if (addedRange == solLen) addedRange = solLen - 1;

                //int rP = lP + addedRange;
                //rP %= solLen;

                solution.PullTowardsSolution(neighbouringSolutions[i], attractionForces[i], neighbouringSolutions, attractionForces, k, exploration);
                //lP = rP + 1;
                //lP %= solLen;
            }

            return solution;
        }

        public ISolution Repulsion(ISolution solution, ISolution[] neighbouringSolutions, bool exploration)
        {
            // Determine attraction forces for each neighbouring solution
            int d = neighbouringSolutions.Length;
            double[] attractionForces = new double[d];
            //int solLen = solution.GetSolutionLength();
            double solutionCharge = GetSolutionCharge(solution);

            for (int i = 0; i < d; i++)
            {
                double charge = GetSolutionCharge(neighbouringSolutions[i]);
                attractionForces[i] = (solutionCharge * charge) / Math.Pow(solution.GetDistanceFromSolution(neighbouringSolutions[i]), 2);
            }

            for (int i = 0; i < d; i++)
            {
                solution.RepelFromSolution(neighbouringSolutions[i], attractionForces[i], neighbouringSolutions, attractionForces, k, exploration);
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
