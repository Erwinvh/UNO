using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using SharedDataClasses;

namespace Server
{
    public class Lobby
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

        //
        //--Resets player's ready state to false--
        //
        public void resetReady()
        {
            foreach(User player in players)
            {
                player.isReady = false;
            }

            gameSession = null;
        }

        //
        //--Starts a new game from lobby--
        //
        public bool startGame()
        {
            if (players.Count>=2)
            {
                gameSession = new Game(server, this);
                return true;
            }
            return false;
        }

        //
        //--Adds player to the lobby--
        //
        public void playerJoin(string username)
        {
            players.Add(new User(username));
        }

        //
        //--Handles a player quitting the lobby--
        //
        public void playerQuit(string username)
        {
            Console.WriteLine("We are removing a player from the lobby");
            
            if (gameSession != null)
            {
                gameSession.playerQuitCase(username);
            }
            players.Remove(getUser(username));
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

        //
        //--Send a message to all clients currentle joined in the lobby--
        //
        public void sendToAll(string message)
        {
            foreach (User player in players)
            {
                server.SendClientMessage(player.name, message);
            }
        }

        //
        //--Gets a user from the lobbys player list--
        //
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

        //
        //--Sets a player's ready state to true--
        //
        public void ToggleReady(string name)
        {
            getUser(name).isReady = !getUser(name).isReady;
            if (checkGameReady())
            {
                startGame();
            }
        }

        //
        //--Checks if all players in lobby are ready--
        //
        public bool checkGameReady()
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
