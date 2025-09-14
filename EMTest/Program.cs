using ElectromagneticAlgorithm;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using static ElectromagneticAlgorithm.AlgorithmUtils;

public class EMTest
{
    public static void Main()
    {
        string exeDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        Console.WriteLine(exeDirectory);
        Console.WriteLine("Podaj nazwę instancji danych: ");
        string dSet = Console.ReadLine();
        string dataPath = exeDirectory + @"\Input\" + dSet + ".dat";

        SolutionQAP.SetQAPData(dataPath);
        int solutionLength = SolutionQAP.solutionLength;
        Console.WriteLine($"Wymiarowość problemu {SolutionQAP.solutionLength}");
        Console.WriteLine($"Średni koszt rozwiązania: {SolutionQAP.GetAverageCost()}");


        Console.WriteLine("Ile razy wywołać algorytm?: ");
        int noAlgRuns = int.Parse(Console.ReadLine());

        Console.WriteLine("Podaj ziarno losowania populacji początkowej: ");
        int seed = int.Parse(Console.ReadLine());

        Console.WriteLine("Co ile wywołań zmienić ziarno? (inkrementacja o 10000): ");
        int incrementSeed = int.Parse(Console.ReadLine());
        int incrementSeedVal = 10000;

        Console.WriteLine("Wybierz typ: 1 - std, 2 - PMX1, 3 - PMX2, 4 - RepCEV: ");
        int nType = int.Parse(Console.ReadLine());

        Console.WriteLine("Podaj rozmiar populacji: ");
        int initialPopulationSize = int.Parse(Console.ReadLine());

        ISolution[] initialPopulation = CreateInitialPopulationForQAP(solutionLength, initialPopulationSize, nType, seed);

        Console.WriteLine($"Wybrana klasa: {initialPopulation[0].GetType()}");

        Console.WriteLine("Podaj liczbę iteracji: ");
        int maxIter = int.Parse(Console.ReadLine());

        Console.WriteLine("Podaj liczbę iteracji w jednym cyklu: ");
        int cycleIter = int.Parse(Console.ReadLine());
        int bonusExploatationIter = 0;

        Console.WriteLine("Podaj odległość sąsiedztwa: ");
        int neighbourhoodDistance = int.Parse(Console.ReadLine());

        Console.WriteLine("Podaj maksymalną liczebność sąsiedztwa: ");
        int maxNeighbourhoodSize = int.Parse(Console.ReadLine());

        Console.WriteLine("Podaj prawdopodobieństwo przyciągania: ");
        double attractionProbability = double.Parse(Console.ReadLine());
        float subsetRatio = 1.0f;

        Console.WriteLine("Podaj prawdopodobieństwo przeszukiwania lokalnego (tylko przy eksploitacji): ");
        double localSearchProb = double.Parse(Console.ReadLine());

        Console.WriteLine("Podaj parametr k dla CEV: ");
        int k = int.Parse(Console.ReadLine());

        Console.WriteLine("Podaj minimalną entropię: ");
        double entropyMin = double.Parse(Console.ReadLine());
        Console.WriteLine("Podaj maksymalną entropię: ");
        double entropyMax = double.Parse(Console.ReadLine());

        /*double entropyMin, entropyMax;
        if (initialPopulationSize == 200 && (dSet == "chr22a" || dSet == "chr22b"))
        {
            entropyMin = 7.49178; // For 200 pop
            entropyMax = 7.64195; // For 200 pop
        }
        else
        {
            Console.WriteLine("Podaj minimalną entropię: ");
            entropyMin = double.Parse(Console.ReadLine());
            Console.WriteLine("Podaj maksymalną entropię: ");
            entropyMax = double.Parse(Console.ReadLine());
        }*/

        Console.WriteLine("Inicjalizować zapisywanie przebiegów? (t/n): ");
        bool dataSaveInit = Console.ReadLine() == "t";
        string dataSaverPath = exeDirectory + @"\Output";

        //solver.PrintPopulation();
        (double bestSolution, long timeMs)[] algOutputs = new (double bestSolution, long timeMs)[noAlgRuns];
        for (int i = 0; i < noAlgRuns; i++)
        {
            EMSolver solver = new(initialPopulation, maxIter, cycleIter, bonusExploatationIter, neighbourhoodDistance, maxNeighbourhoodSize, attractionProbability, subsetRatio, k, localSearchProb, entropyMin, entropyMax);
            if (dataSaveInit)
                solver.InitializeBestSolutionDataSaver(dataSaverPath, i);

            //solver.PrintPopulation();

            (ISolution bestSolution, long timeMs) result = solver.RunAlgorithm();
            Console.WriteLine(result.bestSolution.GetCost());
            Console.WriteLine(((SolutionQAP)result.bestSolution).ToString());
            algOutputs[i] = (result.bestSolution.GetCost(), result.timeMs);

            // Nowa populacja początkowa
            if ((i + 1) % incrementSeed == 0)
            {
                Console.WriteLine("New init pop");
                seed += incrementSeedVal;
                initialPopulation = CreateInitialPopulationForQAP(solutionLength, initialPopulationSize, nType, seed);
            }
        }

        //solver.PrintPopulation();
        Console.WriteLine("\nAlg outputs: ");
        for (int i = 0; i < noAlgRuns; i++)
        {
            Console.WriteLine($"Run {i}: val {algOutputs[i].bestSolution}, time {algOutputs[i].timeMs / 1000.0} sec.");
        }

        (double bestSolution, long timeMs) bestOutput = algOutputs.MinBy(o => o.bestSolution);
        (double bestSolution, long timeMs) worstOutput = algOutputs.MaxBy(o => o.bestSolution);
        double averageOutput = algOutputs.Sum(o => o.bestSolution) / noAlgRuns;
        double averageTime = algOutputs.Sum(o => o.timeMs) / (double)noAlgRuns;

        var bestSolutionOutputs = from el in algOutputs select el.bestSolution;
        var timeOutputs = from el in algOutputs select (double)el.timeMs;

        Console.WriteLine($"\nBest of all: val {bestOutput.bestSolution}, time {bestOutput.timeMs / 1000.0} sec.");
        Console.WriteLine();
        Console.WriteLine($"Worst of all: val {worstOutput.bestSolution}, time {worstOutput.timeMs / 1000.0} sec.");

        Console.WriteLine($"\nAverage output: {averageOutput}, average time {averageTime / 1000.0} sec.");
        Console.WriteLine($"\nstd output: {CalculateStandardDeviation(bestSolutionOutputs)}, std time {CalculateStandardDeviation(timeOutputs) / 1000.0} sec.");

        Console.ReadLine();
    }
}
