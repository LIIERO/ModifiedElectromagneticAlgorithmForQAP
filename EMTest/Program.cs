using ElectromagneticAlgorithm;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

public class EMTest
{
    public static void Main()
    {
        string exeDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        Console.WriteLine(exeDirectory);
        //string pathStart = @"E:\SzkolaProgramowanie\Magisterka\ModifiedElectromagneticAlgorithmForQAP\Data\chr22";
        Console.WriteLine("Podaj zbiór danych (a/b): ");
        string dSet = Console.ReadLine();
        string dataPath = exeDirectory + @"\Input\chr22" + dSet + ".dat";

        //string dataPath = @"E:\SzkolaProgramowanie\Magisterka\ModifiedElectromagneticAlgorithmForQAP\Data\chr22b.dat";
        SolutionQAP.SetQAPData(dataPath);
        int solutionLength = SolutionQAP.solutionLength;
        Console.WriteLine($"Wymiarowość problemu {SolutionQAP.solutionLength}");
        Console.WriteLine($"Średni koszt rozwiązania: {SolutionQAP.GetAverageCost()}");


        //// Random search
        //SolutionQAP sol = new();
        //sol.SetSolutionRepresentation<List<int>>(Enumerable.Range(0, solutionLength).ToList());
        //RandomSearchQAP rs = new(sol);
        //rs.Search(260000);
        //return;


        /*// Test PMX
        SolutionQAP_PMX2 s1 = new(); SolutionQAP_PMX2 s2 = new();
        s1.SetSolutionRepresentation(new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 10, 11 });
        s2.SetSolutionRepresentation(new List<int> { 4, 5, 2, 1, 8, 7, 6, 9, 3, 11, 10, 0 });

        s1.PullTowardsSolution(s2, 1.0, new SolutionQAP_PMX2[] { }, new double[] { 1.0 }, 3, false);

        Console.WriteLine(s1.ToString());
        Console.WriteLine(s2.ToString());
        Console.WriteLine(s1.GetCost());

        Console.ReadLine();*/

        /*// Test ChooseRandom
        int[] ints = new int[] { 2, 1, 3, 7, 9, 8, 4, 5, 6, 0 };
        int[] subset = AlgorithmUtils.ChooseRandom(ints, 4);
        foreach (int i in subset) Console.WriteLine(i);*/

        // Test conditional value
        //int[] c = [int.MaxValue, 3, 6, 2, int.MaxValue, int.MaxValue, 5, 11, 9, int.MaxValue, int.MaxValue, 0];
        //double expVal = SolutionQAP.GetConditionalExpectedCost(c);
        //Console.WriteLine(expVal);


        // Inicjalizacja =================================================================================
        Console.WriteLine("Ile razy wywołać algorytm?: ");
        int noAlgRuns = int.Parse(Console.ReadLine());

        Console.WriteLine("Podaj ziarno losowania populacji początkowej: ");
        int seed = int.Parse(Console.ReadLine());

        Console.WriteLine("Co ile wywołań zmienić ziarno? (inkrementacja o 10000): ");
        int incrementSeed = int.Parse(Console.ReadLine());
        int incrementSeedVal = 10000;


        Console.WriteLine("Wybierz typ: 1 - std, 2 - PMX1, 3 - PMX2, 4 - RepCEV: ");
        int nType = int.Parse(Console.ReadLine());

        // Tworzenie początkowej populacji
        //const int calculatedPopulationSize = 200;
        Console.WriteLine("Podaj rozmiar populacji: ");
        int initialPopulationSize = int.Parse(Console.ReadLine());

        ISolution[] initialPopulation = AlgorithmUtils.CreateInitialPopulationForQAP(solutionLength, initialPopulationSize, nType, seed);

        Console.WriteLine($"Wybrana klasa: {initialPopulation[0].GetType()}");

        // Inicjalizacja algorytmu elektromagnetycznego
        Console.WriteLine("Podaj liczbę iteracji: ");
        int noIter = int.Parse(Console.ReadLine());
        int maxIter = noIter;
        int cycleIter = noIter;
        int bonusExploatationIter = 0;

        Console.WriteLine("Podaj odległość sąsiedztwa: ");
        int neighbourhoodDistance = int.Parse(Console.ReadLine()); // 18

        Console.WriteLine("Podaj maksymalną liczebność sąsiedztwa: ");
        int maxNeighbourhoodSize = int.Parse(Console.ReadLine()); // 15

        Console.WriteLine("Podaj prawdopodobieństwo przyciągania: ");
        double attractionProbability = double.Parse(Console.ReadLine()); // 0.8
        float subsetRatio = 1.0f; // 1

        Console.WriteLine("Podaj parametr k dla CEV: ");
        int k = int.Parse(Console.ReadLine()); // 3

        double entropyMin, entropyMax;
        if (initialPopulationSize == 200)
        {
            entropyMin = 7.54963; // For 200 pop
            entropyMax = 7.64195; // For 200 pop
        }
        else
        {
            Console.WriteLine("Podaj minimalną entropię: ");
            entropyMin = double.Parse(Console.ReadLine());
            Console.WriteLine("Podaj maksymalną entropię: ");
            entropyMax = double.Parse(Console.ReadLine());
        }

        Console.WriteLine("Inicjalizować zapisywanie przebiegów? (t/n): ");
        bool dataSaveInit = Console.ReadLine() == "t";

        //string dataSaverPath = "E:\\SzkolaProgramowanie\\Magisterka\\AlgorithmOutput\\bestSolutionData.csv";
        string dataSaverPath = exeDirectory + @"\Output";


        //solver.PrintPopulation();
        (double bestSolution, long timeMs)[] algOutputs = new (double bestSolution, long timeMs)[noAlgRuns];
        for (int i = 0; i < noAlgRuns; i++)
        {
            EMSolver solver = new(initialPopulation, maxIter, cycleIter, bonusExploatationIter, neighbourhoodDistance, maxNeighbourhoodSize, attractionProbability, subsetRatio, k, entropyMin, entropyMax);
            if (dataSaveInit)
                solver.InitializeBestSolutionDataSaver(dataSaverPath, i);

            solver.PrintPopulation();

            (double bestSolution, long timeMs) result = solver.RunAlgorithm();
            algOutputs[i] = result;

            // Make new initial population
            if ((i + 1) % incrementSeed == 0)
            {
                Console.WriteLine("New init pop");
                seed += incrementSeedVal;
                initialPopulation = AlgorithmUtils.CreateInitialPopulationForQAP(solutionLength, initialPopulationSize, nType, seed);
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
        Console.WriteLine($"Worst of all: val {worstOutput.bestSolution}, time {worstOutput.timeMs / 1000.0} sec.");

        Console.WriteLine($"\nAverage output: {averageOutput}, average time {averageTime / 1000.0} sec.");
        Console.WriteLine($"\nstd output: {AlgorithmUtils.CalculateStandardDeviation(bestSolutionOutputs)}, std time {AlgorithmUtils.CalculateStandardDeviation(timeOutputs) / 1000.0} sec.");

        Console.ReadLine();



        /*int initialPopulationSize = 200;
        SolutionQAP_PMX2[] initialPopulation = new SolutionQAP_PMX2[initialPopulationSize];

        for (int i = 0; i < initialPopulationSize; i++)
        {
            initialPopulation[i] = new SolutionQAP_PMX2();

            List<int> newSolution = Enumerable.Range(0, solutionLength).ToList();
            AlgorithmUtils.Shuffle(newSolution);
            initialPopulation[i].SetSolutionRepresentation(newSolution);

        }

        int maxIter = 100;
        int smax = 15;
        int neighbourhoodDistance = 18;
        double attractionProbability = 0.8;
        double entropyMin = 7.54963; // For 200 pop
        double entropyMax = 7.64195; // For 200 pop

        EMSolver solver = new(initialPopulation, maxIter, maxIter, 0, neighbourhoodDistance, smax, attractionProbability, 1.0f, 4, entropyMin, entropyMax);
        solver.InitializeBestSolutionDataSaver(exeDirectory + @"\Output", 0);

        solver.PrintPopulation();
        (double bestSolution, long timeMs) result = solver.RunAlgorithm();
        solver.PrintPopulation();
        Console.WriteLine(result.bestSolution);
        Console.ReadLine();*/
    }
}
