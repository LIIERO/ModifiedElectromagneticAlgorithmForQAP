using ElectromagneticAlgorithm;

public class EMTest
{
    public static void Main()
    {
        // Inicjalizacja problemu QAP danymi
        int[][] facilityFlows = [
            [ 0, 2, 3, 1 ],
            [ 2, 0, 1, 4 ],
            [ 3, 1, 0, 2 ],
            [ 1, 4, 2, 0 ]
        ];
        int[][] locationDistances = [
            [ 0, 1, 2, 3 ],
            [ 1, 0, 4, 2 ],
            [ 2, 4, 0, 1 ],
            [ 3, 2, 1, 0 ]
        ];
        SolutionQAP.SetQAPData(facilityFlows, locationDistances);
        int solutionLength = facilityFlows.Length;


        // Test PMX
        /*SolutionQAP s1 = new(); SolutionQAP s2 = new();
        s1.SetSolutionRepresentation(new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
        s2.SetSolutionRepresentation(new List<int> { 4, 5, 2, 1, 8, 7, 6, 9, 3 });

        EMSolver.PMX(s1, s2, 3, 7);

        Console.WriteLine(s1.ToString());
        Console.WriteLine(s2.ToString());

        Console.ReadLine();*/



        // Tworzenie początkowej populacji
        int initialPopulationSize = 20;
        SolutionQAP[] initialPopulation = new SolutionQAP[initialPopulationSize];

        for (int i = 0; i < initialPopulationSize; i++)
        {
            initialPopulation[i] = new SolutionQAP();
            // TODO: W taki czy inny sposób zainicjalizuj populację początkową, metodę która to robi możesz przenieść do AlgorithmUtils
            // Na razie jest losowo
            List<int> newSolution = Enumerable.Range(0, solutionLength).ToList();
            AlgorithmUtils.Shuffle(newSolution);

            initialPopulation[i].SetSolutionRepresentation(newSolution);
        }



        // Inicjalizacja algorytmu elektromagnetycznego

        // TODO: Parametry
        int maxIter = 0;
        int maxLocalIter = 0;

        EMSolver solver = new(initialPopulation, maxIter, maxLocalIter);
        solver.PrintPopulation();


        Console.ReadLine();
    }
}
