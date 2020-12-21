using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
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

        public void resetReady()
        {
            foreach(User player in players)
            {
                player.isReady = false;
            }

            gameSession = null;
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
            Console.WriteLine("We are removing a player from the lobby");
            
            if (gameSession != null)
            {
                gameSession.playerQuitCase(username);
            }
            players.Remove(getUser(username));
            //TODO: remove all player stuff
            if (players.Count==0)
            {
                server.lobbyList.Remove(this);
                return;
            }

            foreach (User player in players)
            {
                LobbyMessage lm = new LobbyMessage(username, "");
                server.SendClientMessage(player.name, JsonSerializer.Serialize(lm));
            }
        }

        public void sendToAll(string message)
        {
            foreach (User player in players)
            {
                server.SendClientMessage(player.name, message);
            }
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
