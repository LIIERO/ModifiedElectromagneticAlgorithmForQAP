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

        // Tworzenie początkowej populacji
        List<IPermutationSolution> initialPopulation = new()
        {
            new SolutionQAP(),
            new SolutionQAP(),
            new SolutionQAP()
        };

        // Inicjalizacja algorytmu elektromagnetycznego
        EMSolver solver = new(initialPopulation);

        Console.ReadLine();
    }
}
