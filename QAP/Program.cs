using System.Collections.Generic;
using System.Linq;

class FacilityLocation
{
    // Na wikipedii bijekcja (assignment) przechodzi ze zbioru fabryk do zbioru lokacji, a tu i w pracy prof. Chmiela jest na odwrót
    // Bijekcję zapisujemy jako permutację indeksów fabryk. Indeks to indeks lokacji, a wartość to indeks fabryki

    static int CalculateTotalCost(List<List<int>> facilities, List<List<int>> locations, List<int> assignment)
    {
        int totalCost = 0;
        int n = facilities.Count;

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                int facility1 = assignment[i];
                int facility2 = assignment[j];
                int location1 = i;
                int location2 = j;

                totalCost += facilities[facility1][facility2] * locations[location1][location2];
            }
        }

        return totalCost;
    }

    static void Main()
    {
        // Facilities cost matrix
        List<List<int>> facilities = new List<List<int>>
        {
            new List<int> { 0, 2, 3, 1 },
            new List<int> { 2, 0, 1, 4 },
            new List<int> { 3, 1, 0, 2 },
            new List<int> { 1, 4, 2, 0 }
        };

        // Flow matrix
        List<List<int>> locations = new List<List<int>>
        {
            new List<int> { 0, 1, 2, 3 },
            new List<int> { 1, 0, 4, 2 },
            new List<int> { 2, 4, 0, 1 },
            new List<int> { 3, 2, 1, 0 }
        };

        int n = facilities.Count;

        // Generate initial assignment (0, 1, 2, 3)
        List<int> assignment = Enumerable.Range(0, n).ToList();

        // Calculate the initial total cost
        int minCost = CalculateTotalCost(facilities, locations, assignment);
        List<int> minAssignment = assignment;

        // Generate all permutations of the assignment
        while (NextPermutation(assignment))
        {
            int cost = CalculateTotalCost(facilities, locations, assignment);
            if (cost < minCost)
            {
                minCost = cost;
                minAssignment = assignment.ToList();
            }
        }

        // Print the optimal assignment and total cost
        Console.Write("Optimal Assignment: ");
        for (int i = 0; i < n; i++)
        {
            Console.Write($"{minAssignment[i]}, ");
        }
        Console.WriteLine();
        for (int i = 0; i < n; i++)
        {
            Console.Write($"F{minAssignment[i] + 1}->L{i + 1} ");
        }
        Console.WriteLine();
        Console.WriteLine($"Total Cost: {minCost}");

        Console.ReadLine();
    }

    static bool NextPermutation(List<int> list)
    {
        int i = list.Count - 2;
        while (i >= 0 && list[i] >= list[i + 1])
        {
            i--;
        }
        if (i < 0)
        {
            return false;
        }

        int j = list.Count - 1;
        while (list[j] <= list[i])
        {
            j--;
        }

        int temp = list[i];
        list[i] = list[j];
        list[j] = temp;

        Reverse(list, i + 1, list.Count - 1);
        return true;
    }

    static void Reverse(List<int> list, int start, int end)
    {
        while (start < end)
        {
            int temp = list[start];
            list[start] = list[end];
            list[end] = temp;
            start++;
            end--;
        }
    }
}