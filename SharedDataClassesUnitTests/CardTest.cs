using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedDataClasses;

namespace SharedDataClassesUnitTests
{
    [TestClass]
    public class CardTest
    {
        private const string Expected = "/Cards/red9.png";
        private const Card.Color ExpectedColor = Card.Color.RED;

        [TestMethod]
        public void getsourcepathTest()
        {
            Card testingCard = new Card(Card.Color.RED, 9);

            testingCard.GetSourcepath();
            Assert.AreEqual(Expected, testingCard.SourcePath);
        }

        [TestMethod]
        public void setColorTest()
        {
            Card testingCard = new Card(Card.Color.BLUE, 9);

            testingCard.setColor(Card.Color.RED);
            Assert.AreEqual(Expected, testingCard.SourcePath);
            Assert.AreEqual(ExpectedColor, testingCard.color);
        }

    }
}
