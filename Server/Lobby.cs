using System;
using System.Collections.Generic;
using System.Text;
using SharedDataClasses;

namespace Server
{
    class Lobby
    {
        public string LobbyCode { get; set; }
        public List<string> players { get; set; }
        private Server server { get; set; }
        public Game gameSession { get; set; }

        public Lobby(string username, string lobbyCode, Server server)
        {
            LobbyCode = lobbyCode;
            players = new List<string>();
            players.Add(username);
            this.server = server;
        }

        public bool startGame()
        {
            if (players.Count>2)
            {
                gameSession = new Game(server);
                return true;
            }
            return false;
        }

        public void playerJoin(string username)
        {
            players.Add(username);
        }

        public void playerQuit(string username)
        {
            players.Remove(username);
        }
    }
}
