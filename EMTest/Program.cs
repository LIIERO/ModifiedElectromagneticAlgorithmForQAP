using ElectromagneticAlgorithm;

public class EMTest
{
    public static void Main()
    {

        string dataPath = @"E:\SzkolaProgramowanie\Magisterka\ModifiedElectromagneticAlgorithmForQAP\Data\chr12a.dat";
        SolutionQAP.SetQAPData(dataPath);
        int solutionLength = SolutionQAP.solutionLength;
        Console.WriteLine(SolutionQAP.solutionLength);
        Console.WriteLine(SolutionQAP.GetAverageCost());

        /*// Test PMX
        SolutionQAP s1 = new(); SolutionQAP s2 = new();
        s1.SetSolutionRepresentation(new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 10, 11 });
        s2.SetSolutionRepresentation(new List<int> { 4, 5, 2, 1, 8, 7, 6, 9, 3, 11, 10, 0 });

        s1.CrossoverWithSolution(s2, 6, 0);

        Console.WriteLine(s1.ToString());
        Console.WriteLine(s2.ToString());

        Console.ReadLine();*/


        /*// Test ChooseRandom
        int[] ints = new int[] { 2, 1, 3, 7, 9, 8, 4, 5, 6, 0 };
        int[] subset = AlgorithmUtils.ChooseRandom(ints, 4);
        foreach (int i in subset) Console.WriteLine(i);*/


        // Test conditional value
        int[] c = [int.MaxValue, 3, 6, 2, int.MaxValue, int.MaxValue, 5, 11, 9, int.MaxValue, int.MaxValue, 0];
        double expVal = SolutionQAP.GetConditionalExpectedCost(c);
        Console.WriteLine(expVal);

        /*// Tworzenie początkowej populacji
        int initialPopulationSize = 100;
        SolutionQAP[] initialPopulation = new SolutionQAP[initialPopulationSize];

        for (int i = 0; i < initialPopulationSize; i++)
        {
            initialPopulation[i] = new SolutionQAP();
            // TODO: W taki czy inny sposób zainicjalizuj populację początkową, metodę która to robi możesz przenieść do AlgorithmUtils
            // Na razie jest losowo
            List<int> newSolutionRepr = Enumerable.Range(0, solutionLength).ToList();
            AlgorithmUtils.Shuffle(newSolutionRepr);

            initialPopulation[i].SetSolutionRepresentation(newSolutionRepr);
        }


        // Inicjalizacja algorytmu elektromagnetycznego

        // TODO: Parametry
        int maxIter = 500;
        int activeSolutionSampleSize = 10; // TODO: Nieużyte
        int neighbourhoodDistance = 50;
        double attractionProbability = 0.5;

        EMSolver solver = new(initialPopulation, maxIter, activeSolutionSampleSize, neighbourhoodDistance, attractionProbability);
        solver.InitializeBestSolutionDataSaver("E:\\SzkolaProgramowanie\\Magisterka\\AlgorithmOutput\\bestSolutionData.csv");

        solver.PrintPopulation();
        solver.RunAlgorithm();
        solver.PrintPopulation();

        Console.ReadLine();*/
    }
}
