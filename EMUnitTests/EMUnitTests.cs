using ElectromagneticAlgorithm;


namespace EMUnitTests
{
    [TestClass]
    public class EMUnitTests
    {
        [TestMethod]
        public void PMXTest()
        {
            int[][] facilityFlows = [[]];
            int[][] locationDistances = [[]];
            SolutionQAP.SetQAPData(facilityFlows, locationDistances);

            List<int> ex1 = new List<int>() { 1, 2, 3, 1, 8, 7, 6, 8, 9 };
            List<int> ex2 = new List<int>() { 4, 5, 2, 4, 5, 6, 7, 9, 3 };

            SolutionQAP s1 = new(); SolutionQAP s2 = new();
            s1.SetSolutionRepresentation(new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            s2.SetSolutionRepresentation(new List<int> { 4, 5, 2, 1, 8, 7, 6, 9, 3 });

            EMSolver.PMX(s1, s2, 3, 7);

            Assert.AreEqual(ex1, s1.GetSolutionRepresentation());
            Assert.AreEqual(ex2, s2.GetSolutionRepresentation());
        }
    }
}