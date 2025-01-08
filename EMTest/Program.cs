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



        /*SolutionQAP ex1 = new(); SolutionQAP ex2 = new();
        ex1.SetSolutionRepresentation(new List<int> { 1, 2, 3, 1, 8, 7, 6, 8, 9 });
        ex2.SetSolutionRepresentation(new List<int> { 4, 5, 2, 4, 5, 6, 7, 9, 3 });*/

        SolutionQAP s1 = new(); SolutionQAP s2 = new();
        s1.SetSolutionRepresentation(new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
        s2.SetSolutionRepresentation(new List<int> { 4, 5, 2, 1, 8, 7, 6, 9, 3 });

        EMSolver.PMX(s1, s2, 3, 7);

        Console.WriteLine(s1.ToString());
        Console.WriteLine(s2.ToString());

        Console.ReadLine();

        

        // Tworzenie początkowej populacji
        List<IPermutationSolution> initialPopulation = new()
        {
            new SolutionQAP(),
            new SolutionQAP(),
            new SolutionQAP()
        };

        // Inicjalizacja algorytmu elektromagnetycznego
        //EMSolver solver = new(initialPopulation);

        Console.ReadLine();
    }
}
