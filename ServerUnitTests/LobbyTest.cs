using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server;
using SharedDataClasses;

namespace ServerUnitTests
{
    [TestClass]
    public class LobbyTest
    {
        [TestMethod]
        public void PlayerJoinTest()
        {
            Lobby testLobby = new Lobby("tester", "TestLobbyCode", null);
            testLobby.playerJoin("Marco");

            bool expected = true;
            bool resolved = false;

            foreach(User u in testLobby.players)
            {
                if (u.name.Equals("Marco"))
                {
                    resolved = true;
                }
            }

            Assert.AreEqual(expected, resolved);

        }

        [TestMethod]
        public void CheckGameReadyTest()
        {
            Lobby testLobby = new Lobby("tester", "TestLobbyCode", null);
            testLobby.playerJoin("Marco");

            bool expected = false;
            bool resolved = testLobby.checkGameReady();

            Assert.AreEqual(expected, resolved);

            testLobby.ToggleReady("tester");

            resolved = testLobby.checkGameReady();

            Assert.AreEqual(expected, resolved);

        }

        [TestMethod]
        public void ToggleReadyTest()
        {
            Lobby testLobby = new Lobby("tester", "TestLobbyCode", null);

            bool expected = true;

            testLobby.ToggleReady("tester");
            bool resolved = testLobby.getUser("tester").isReady;

            Assert.AreEqual(expected, resolved);
        }

        [TestMethod]
        public void GetUserTest()
        {
            Lobby testLobby = new Lobby("tester", "TestLobbyCode", null);

            string expectedName = "tester";
            int expectedAmountOfCards = 7;
            bool expectedIsReady = false;

            Assert.AreEqual(expectedName, testLobby.getUser("tester").name);
            Assert.AreEqual(expectedAmountOfCards, testLobby.getUser("tester").amountOfCards);
            Assert.AreEqual(expectedIsReady, testLobby.getUser("tester").isReady);
        }

        [TestMethod]
        public void ResetReadyTest()
        {
            Lobby testLobby = new Lobby("tester", "TestLobbyCode", null);
            testLobby.playerJoin("Marco");

            testLobby.ToggleReady("Marco");

            bool expectedMarco = false;
            bool expectedTester = false;

            testLobby.resetReady();

            Assert.AreEqual(expectedMarco, testLobby.getUser("Marco").isReady);
            Assert.AreEqual(expectedTester, testLobby.getUser("tester").isReady);

        }
    }
}
