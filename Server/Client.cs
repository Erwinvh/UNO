﻿using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json.Linq;
using SharedDataClasses;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Server
{
    class Client
    {
        private TcpClient tcpClient;
        private NetworkStream stream;
        private Server server;
        public User user;
        public Lobby lobby;

        public Client(TcpClient tcpClient, Server server)
        {
            this.tcpClient = tcpClient;
            this.server = server;
            Thread listernerThread = new Thread(() => Listener());
            listernerThread.Start();
        }

        //
        //--Incomingt data--
        //
        
        private void Listener()
        {
            Byte[] lengtebytes = new byte[2];
            while (true)
            {
                for (int i = 0; i < 2; i++)
                {
                    lengtebytes[i] = (byte)stream.ReadByte();
                    Console.WriteLine(lengtebytes[i]);
                }
                uint bytes = BitConverter.ToUInt16(lengtebytes);
                bytes += 2;
                Byte[] bytebuffer = new byte[bytes];
                for (int i = 0; i < bytes; i++)
                {
                    bytebuffer[i] = (byte)stream.ReadByte();
                    Console.WriteLine(bytebuffer[i]);
                }
                Console.WriteLine("Received packet");
                handleData(Encoding.ASCII.GetString(bytebuffer));
            }
        }

        public void handleData(string packetData)
        {
            Console.WriteLine($"Got a packet: {packetData}");
            JObject pakket = JObject.Parse(packetData);
            MessageID messageId;
            Enum.TryParse((string)pakket.GetValue("MessageID"), out messageId);

            switch (messageId)
            {
                case MessageID.LOBBY:
                    string username = (string) pakket.GetValue("Username");
                    string LobbyCode = (string) pakket.GetValue("LobbyCode");
                    if (server.CheckUsers(username))
                    {
                        user = new User(username);
                        //TODO: add username to user and continue
                        if (server.LobbyExist(LobbyCode))
                        {
                            if (server.LobbyFill(LobbyCode))
                            {
                                //TODO: send systemmessage for full lobby
                            }
                            else
                            {
                                server.addUsertoLobby(username, LobbyCode);
                                lobby = server.GetLobbybyCode(LobbyCode);
                            }
                        }
                        else
                        {
                            lobby = new Lobby(username, LobbyCode, server);
                            server.lobbyList.Add(lobby);
                        }
                    }
                    else
                    {
                        sendSystemMessage(22222222);
                        //TODO: send system message for already in use name
                    }
                    break;
                case MessageID.CHAT:
                    Broadcast(packetData);
                    sendSystemMessage(101);
                    break;
                case MessageID.GAME:
                    //TODO: switch case: startgame, left game, ready
                    break;
                case MessageID.MOVE:
                    JObject JCard = (JObject)pakket.GetValue("playedCard");
                    Card playedCard = JCard.ToObject<Card>();
                    MoveMessage move;
                    if (!lobby.gameSession.checkMove(playedCard))
                    {
                        move = new MoveMessage(lobby.gameSession.drawCard(user.name), user.name, true);
                    }
                    else
                    {
                        move = new MoveMessage(playedCard, user.name);
                    }
                    sendSystemMessage(101);
                    Broadcast(JsonSerializer.Serialize(move));
                    Broadcast(JsonSerializer.Serialize(lobby.gameSession.GeneratePlayerStatusMessage()));
                    if (lobby.gameSession.Checkhand())
                    {
                            GameMessage EGM = new GameMessage(user.name, "Win");
                            Broadcast(JsonSerializer.Serialize(EGM));
                            foreach (Client client in server.clients)
                            {
                                Score score = server.fileSystem.getScoreByUser(client.user.name);
                                score.gameAmount++;
                                if (score.username == user.name)
                                {
                                    score.winAmount++;
                                }
                                server.fileSystem.updateScore(score);
                            }
                            server.fileSystem.WritetoFile();
                    }else if (lobby.gameSession.checkUNO())
                    {
                            GameMessage gm = new GameMessage(user.name, "UNO!");
                            Broadcast(JsonSerializer.Serialize(gm));
                    }
                    TurnMessage turn = lobby.gameSession.GenerateTurn(false);
                    Broadcast(JsonSerializer.Serialize(turn));
                    if (turn.addedCards.Count>0)
                    {
                        TurnMessage forfeitTurnMessage = lobby.gameSession.GenerateTurn(true);
                        Broadcast(JsonSerializer.Serialize(forfeitTurnMessage));
                    }
                    break;
            }
        }

       



        //
        //--Outgoing data
        //
 public void Broadcast(string pakketdata)
        {
            foreach (Client client in server.clients)
            {
                Write(pakketdata);
            }
        }
     public void sendSystemMessage(int message)
        {
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

    }
}
