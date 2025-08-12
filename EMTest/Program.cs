using ElectromagneticAlgorithm;
using System.IO;
using System.Reflection;

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


        Console.WriteLine("Wybierz typ: 1 - std, 2 - PMX1, 3 - PMX2, 4 - RepCEV: ");
        int nType = int.Parse(Console.ReadLine());

        // Tworzenie początkowej populacji
        int initialPopulationSize = 200; // 200, 300
        ISolution[] initialPopulation = new ISolution[initialPopulationSize];

        for (int i = 0; i < initialPopulationSize; i++)
        {
            switch (nType)
            {
                case 2:
                    initialPopulation[i] = new SolutionQAP_PMX1();
                    break;
                case 3:
                    initialPopulation[i] = new SolutionQAP_PMX2();
                    break;
                case 4:
                    initialPopulation[i] = new SolutionQAP_RepCEV();
                    break;
                default:
                    initialPopulation[i] = new SolutionQAP();
                    break;
            }
            
            List<int> newSolutionRepr = Enumerable.Range(0, solutionLength).ToList();
            AlgorithmUtils.Shuffle(newSolutionRepr);

            initialPopulation[i].SetSolutionRepresentation(newSolutionRepr);
        }


        // Inicjalizacja algorytmu elektromagnetycznego

        // TODO: Parametry
        int maxIter = 100;
        int cycleIter = 100;
        int bonusExploatationIter = 0;
        int neighbourhoodDistance = 18;
        int maxNeighbourhoodSize = 15;
        double attractionProbability = 0.8;
        float subsetRatio = 1.0f; // 1
        int k = 3;
        double entropyMin = 7.54963; // For 200 pop
        double entropyMax = 7.64195; // For 200 pop
        //string dataSaverPath = "E:\\SzkolaProgramowanie\\Magisterka\\AlgorithmOutput\\bestSolutionData.csv";
        string dataSaverPath = exeDirectory + @"\Output\bestSolutionData.csv";

        EMSolver solver = new(initialPopulation, maxIter, cycleIter, bonusExploatationIter, neighbourhoodDistance, maxNeighbourhoodSize, attractionProbability, subsetRatio, k, entropyMin, entropyMax);
        solver.InitializeBestSolutionDataSaver(dataSaverPath);

        solver.PrintPopulation();
        (double bestSolution, long timeMs) result = solver.RunAlgorithm();
        solver.PrintPopulation();

        Console.WriteLine($"\nBest final solution cost: {result.bestSolution}");
        Console.WriteLine($"\nElapsed time (seconds): {result.timeMs / 1000.0}");

        Console.ReadLine();
    }
}
