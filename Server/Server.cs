using SharedDataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Server server = new Server();
        }
    }

    class Server
    {
        //
        //--Coms related--
        //
        private TcpListener listener;
        public List<Client> clients = new List<Client>();

        //
        //--File related--
        //
        public FileSystem fileSystem { get; }

        //
        //--Game related--
        //
        public List<Lobby> lobbyList { get; set; }
        public Dictionary<string, string> UserDictionary { get; set; }

        //The constructor of the server
        public Server()
        {
            //this.fileSystem = fileSystem;
            lobbyList = new List<Lobby>();
            UserDictionary = new Dictionary<string, string>();
            listener = new TcpListener(IPAddress.Any, 15243);
            listener.Start();
            listener.BeginAcceptTcpClient(new AsyncCallback(OnConnect), null);
            Console.WriteLine("Server is online");
            Console.ReadLine();
            
        }

        //The callback method to connect the clients
        private void OnConnect(IAsyncResult ar)
        {
            //TODO: if in game user has to wait to connect, seperate list of clients
            var tcpClient = listener.EndAcceptTcpClient(ar);
            Console.WriteLine($"Client connected from {tcpClient.Client.RemoteEndPoint}");
            clients.Add(new Client(tcpClient, this));
            listener.BeginAcceptTcpClient(new AsyncCallback(OnConnect), null);
        }

        internal void Broadcast(string Data)
        {
            foreach (Client client in clients)
            {
                client.Write(Data);
            }
        }

        //The disconnect method for the clients
        internal void Disconnect(Client client)
        {
            clients.Remove(client);
            Console.WriteLine("Client disconnected");
        }

        internal Client getClient(string name)
        {
            foreach (Client client in clients)
            {
                if (client.user.name == name)
                {
                    return client;
                }
            }

            return null;
        }

        internal void SendClientMessage(string username, string message)
        {
            foreach (Client client in clients)
            {
                if (client.user.name == username)
                {
                    client.Write(message);
                }
            }
        }

        internal bool CheckUsers(string username)
        {
            return !UserDictionary.ContainsKey(username);
        }


        //
        //--Lobby related--
        //
        
        internal bool LobbyExist(string lobbyCode)
        {
            return GetLobbybyCode(lobbyCode) != null;
        }

        public Lobby GetLobbybyCode(string LobbyCode)
        {
            foreach (Lobby lobby in lobbyList)
            {
                if (lobby.LobbyCode == LobbyCode)
                {
                    return lobby;
                }
            }
            return null;
        }

        internal bool LobbyFill(string lobbyCode)
        {
            return GetLobbybyCode(lobbyCode).players.Count>4;
        }

        internal void addUsertoLobby(string username, string lobbyCode)
        {
            Lobby lobby = GetLobbybyCode(lobbyCode);
            foreach (string player in lobby.players)
            {
                SendClientMessage(player, JsonSerializer.Serialize(new LobbyMessage(username, lobbyCode)));
            }
            lobby.playerJoin(username);
            UserDictionary[username] = lobbyCode;
        }
    }
}