using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedDataClasses;

namespace SharedDataClassesUnitTests
{
    [TestClass]
    public class ScoreTest
    {
        private const int Expected = 2;
        [TestMethod]
        public void increaseWinAmountTest()
        {
            
            Score testingScore = new Score("tester", 1 , 2);

            testingScore.increaseWinAmount();
            Assert.AreEqual(Expected, testingScore.winAmount);
        }

        [TestMethod]
        public void increaseGameAmountTest()
        {
            Score testingScore = new Score("tester", 1, 1);

            testingScore.increaseGameAmount();
            Assert.AreEqual(Expected, testingScore.gameAmount);
        }
    }
}
