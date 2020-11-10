using System;
using System.Collections.Generic;
using System.Text;
using SharedDataClasses;

namespace Server
{
    class Lobby
    {
        public string LobbyCode { get; set; }
        public List<User> players { get; set; }
        private Server server { get; set; }
        public Game gameSession { get; set; }

        public Lobby(string username, string lobbyCode, Server server)
        {
            LobbyCode = lobbyCode;
            players = new List<User>();
            players.Add(new User(username));
            this.server = server;
        }

        public bool startGame()
        {
            if (players.Count>=2)
            {
                gameSession = new Game(server, this);
                return true;
            }
            return false;
        }

        public void playerJoin(string username)
        {
            players.Add(new User(username));
        }

        public void playerQuit(string username)
        {
            players.Remove(getUser(username));
            //gameSession.playerQuitCase(username);
            //TODO: remove all player stuff


        }

        public User getUser(string username)
        {
            foreach (User user in players)
            {
                if (user.name==username)
                {
                    return user;
                }
            }

            return null;
        }

        public void ToggleReady(string name)
        {
            getUser(name).isReady = !getUser(name).isReady;
            if (checkGameReady())
            {
                startGame();
            }
        }

        private bool checkGameReady()
        {
            foreach (User player in players)
            {
                if (!player.isReady)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
