﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json.Linq;
using SharedDataClasses;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Server
{
    public class Client
    {
        private TcpClient tcpClient;
        private NetworkStream stream;
        private Server server;
        public User user;
        public Lobby lobby;
        public bool running = true;
        public List<Card> hand { get; set; }

        public Client(TcpClient tcpClient, Server server)
        {
            user = new User(null);
            this.tcpClient = tcpClient;
            this.server = server;
            this.hand = new List<Card>();
            this.stream = this.tcpClient.GetStream();
            Thread listernerThread = new Thread(() => Listener());
            listernerThread.Start();
        }

        //
        //--Incomingt data--
        //
        
        private void Listener()
        {
            Byte[] lengtebytes = new byte[2];
            while (running)
            {
                for (int i = 0; i < 2; i++)
                {
                    lengtebytes[i] = (byte)stream.ReadByte();
                }
                uint bytes = BitConverter.ToUInt16(lengtebytes);
                bytes += 2;
                Byte[] bytebuffer = new byte[bytes];
                for (int i = 0; i < bytes; i++)
                {
                    bytebuffer[i] = (byte)stream.ReadByte();
                }
                Console.WriteLine("Received packet!");                  //TODO!!
                handleData(Encoding.ASCII.GetString(bytebuffer));
            }
            stream.Close();

            tcpClient.Close();

            server.clients.Remove(this);
        }

        //
        //--Handles data coming into the server--
        //
        public void handleData(string packetData)
        {
            Console.WriteLine($"Got a packet: {packetData}");
            JObject pakket = JObject.Parse(packetData);
            MessageID messageId;
            Enum.TryParse((string)pakket.GetValue("MessageID"), out messageId);
            string username = (string) pakket.GetValue("Username");
            Console.WriteLine("username has been set to:" + username + " for message" + messageId.ToString());
            switch (messageId)
            {
                //Case handles player wants to enter a lobby
                case MessageID.LOBBY:
                    string LobbyCode = (string) pakket.GetValue("LobbyCode");

                    if (LobbyCode == "")
                    {
                        foreach (User player in lobby.players)
                        {
                            if (player.name != user.name)
                            {
                                LobbyMessage lm = new LobbyMessage(user.name, ""); 
                                server.SendClientMessage(player.name, JsonSerializer.Serialize(lm));
                                
                            }
                        }
                        Console.WriteLine("Amount of remaining players = " + lobby.players.Count);
                        lobby.playerQuit(user.name);
                        Console.WriteLine("Amount of remaining players = " + lobby.players.Count);

                        if (lobby.players.Count == 0)
                        {
                            server.lobbyList.Remove(lobby);
                            Console.WriteLine("removed Lobby:" + lobby.LobbyCode);
                        }
                        server.UserDictionary[user.name] = "";
                        break;
                    }

                    if (user.name == username && server.UserDictionary[username].Equals(""))
                    {
                        GoToLobby(LobbyCode);
                        break;
                    }
                    
                    if (server.CheckUsers(username))
                    {
                        if (user.name!=null)
                        {
                            server.UserDictionary.Remove(user.name);
                        }
                        user.name = username;
                        server.UserDictionary.Add(username, LobbyCode);
                        Console.WriteLine("USERNAME OK!");
                        sendSystemMessage(101);
                        GoToLobby(LobbyCode);
                    }
                    else
                    {
                        sendSystemMessage(201);
                    }
                    break;

                //Case handles player sends a chatmessage.
                case MessageID.CHAT:
                    Broadcast(packetData);
                    sendSystemMessage(103);
                    break;
                
                //Case handles player setting state to ready or leaving the game
                case MessageID.GAME:
                    string gamemessage = (string) pakket.GetValue("gameMessage");
                    switch (gamemessage)
                    {
                        //case "game finished":
                        //    //TODO Game finished? Fill case?
                        //    break;

                        //Case handles player leaving game
                        case "left Game":
                            if (username == user.name)
                            {
                                Broadcast(JsonSerializer.Serialize(new GameMessage(user.name, "left Game")));
                                lobby.playerQuit(user.name);
                                server.UserDictionary[user.name] = "";
                            }
                            else
                            {
                                Console.WriteLine(username + " player has left");
                            }
                            break;

                        //Case handles player setting state to ready
                        case "ToggleReady":
                            lobby.ToggleReady(user.name);
                            GameMessage GM = new GameMessage(user.name, "ToggleReady");
                            Broadcast(JsonSerializer.Serialize(GM));
                            break;
                    }
                    break;

                //Case handles players move in-game.
                case MessageID.MOVE:
                    MoveMessage MM = pakket.ToObject<MoveMessage>();
                    MoveMessage move;
                    Card playedCard = MM.playedCard;
                    if (playedCard != null)
                    {
                        Console.WriteLine("This card has been spotted in the movemessage:" + playedCard.SourcePath);
                    }

                    if (!lobby.gameSession.checkMove(playedCard, MM.UserName))
                    {
                        move = new MoveMessage(lobby.gameSession.drawCard(user.name), user.name, true);
                    }
                    else
                    {
                        move = new MoveMessage(playedCard, user.name, false);
                    }

                    sendSystemMessage(101);
                    Broadcast(JsonSerializer.Serialize(move));
                    Console.WriteLine("This Card has been spotted to send to the user:" + move.playedCard.SourcePath + "With void: " + move.isVoidMove);
                    Broadcast(JsonSerializer.Serialize(lobby.gameSession.GeneratePlayerStatusMessage()));
                    
                    if (lobby.gameSession.Checkhand())
                    {
                        lobby.gameSession.win(user.name);
                            server.fileSystem.WritetoFile();
                            break;
                    }else if (lobby.gameSession.checkUNO())
                    {
                            GameMessage gm = new GameMessage(user.name, "UNO!");
                            Broadcast(JsonSerializer.Serialize(gm));
                    }
                    TurnMessage turn = lobby.gameSession.GenerateTurn(false);
                    Console.WriteLine("This is the turn message: " + JsonSerializer.Serialize(turn));
                    Broadcast(JsonSerializer.Serialize(turn));
                    if (turn.addedCards.Count>0)
                    {
                        TurnMessage forfeitTurnMessage = lobby.gameSession.GenerateTurn(true);
                        Broadcast(JsonSerializer.Serialize(forfeitTurnMessage));
                    }
                    break;

                // Case handles system messages
                case MessageID.SYSTEM:
                    SystemMessage SM = pakket.ToObject<SystemMessage>();
                    int code = SM.status;
                    Console.WriteLine("We are removing a player from the lobby" + code);
                    if (code == 200)
                    {
                        
                        disconnect();
                    }
                    break;
            }
        }

        //
        //--When called this method will take care of removing a card from a players hand--
        //  Method takes a cards number and color to identify
        //
        internal void RemoveCard(int number, Card.Color color)
        {
            int removingindex = -1;
            foreach (Card card in hand)
            {
                if (card.number == number && color == card.color)
                {
                    removingindex = hand.IndexOf(card);
                } else if (card.number == number && card.color == Card.Color.BLACK)
                {
                    removingindex = hand.IndexOf(card);
                }
            }

            if (removingindex!=-1)
            {
                hand.RemoveAt(removingindex);
            }
        }

        //
        //--This method handles players entering lobby using the lobby used to login--
        //
        private void GoToLobby(string LobbyCode)
        {
            if (server.LobbyExist(LobbyCode))
            {
                if (server.LobbyFill(LobbyCode))
                {
                    sendSystemMessage(202);
                    return;
                }
                if (server.GetLobbybyCode(LobbyCode).gameSession != null)
                {
                    sendSystemMessage(203);
                    return;
                }
                server.addUsertoLobby(user.name, LobbyCode);
                lobby = server.GetLobbybyCode(LobbyCode);
                sendScoreboard();
                sendSystemMessage(102);
                Thread.Sleep(30);
                foreach (User user in lobby.players)
                {
                    if (user.name != this.user.name)
                    {
                        Console.WriteLine("Player sent to new player: " + user.name);
                        LobbyMessage LM = new LobbyMessage(user.name, lobby.LobbyCode);

                        Write(JsonSerializer.Serialize(LM));
                        if (user.isReady)
                        {
                            GameMessage GM = new GameMessage(user.name, "ToggleReady");
                            Write(JsonSerializer.Serialize(GM));
                        }
                    }
                }
                return;
            }
            lobby = new Lobby(user.name, LobbyCode, server);
            server.lobbyList.Add(lobby);
            sendScoreboard();
            sendSystemMessage(102);
            Console.WriteLine("LOBBY OK!");
        }
        //
        //--Handles sending players' scoreboard to the game, so that this can be displayed to other players!--
        //
        private void sendScoreboard()
        {
            List<Score> scores = server.fileSystem.scoreBoard.scoreboard;
            ScoreMessage ScoreMess = new ScoreMessage(scores);
            Write(JsonSerializer.Serialize(ScoreMess));
            Console.WriteLine("sentScoreboard!!!!!!" + scores.Count);
        }













        //

        //--Outgoing data

        //



        public void Broadcast(string pakketdata)
        {
            foreach (User player in lobby.players)
            {
                server.SendClientMessage(player.name, pakketdata);
                
            }
        }
        
        private void sendLobbyPlayers()
        {
            foreach (User player in lobby.players)
            {
                if (player.name != user.name)
                {
                    Write(JsonSerializer.Serialize(new LobbyMessage(player.name, lobby.LobbyCode)));
                }
            }
        }

     public void sendSystemMessage(int message)
        {
            Console.WriteLine(message);
            Write(JsonSerializer.Serialize(new SystemMessage(message)));
        }

        public void Write(string data)
        {
            int length = data.Length;
            Byte[] lengteBytes = BitConverter.GetBytes(length);

            var dataAsBytes = System.Text.Encoding.ASCII.GetBytes(data);
            var z = new Byte[lengteBytes.Length + dataAsBytes.Length];
            lengteBytes.CopyTo(z, 0);
            dataAsBytes.CopyTo(z, lengteBytes.Length);

            stream.Write(z, 0, z.Length);
            stream.Flush();
        }

        public void disconnect()
        {

            server.UserDictionary.Remove(user.name);
            Lobby enteredLobby = null;
            foreach (Lobby lobby in server.lobbyList)
            {
                if (lobby.getUser(user.name)!=null)
                {
                    enteredLobby = lobby;
                    
                }
            }
            if (enteredLobby!=null)
            {
                enteredLobby.playerQuit(user.name);
            }

            
            running = false;

        }
    }
}
