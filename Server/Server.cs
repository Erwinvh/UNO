using SharedDataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }

    class Server
    {
        private TcpListener listener;
        public List<Client> clients = new List<Client>();
        public FileSystem fileSystem { get; }
        public bool isPlaying { get; set; }
        public Game Game { get; set; }

        //The constructor of the server
        public Server()
        {
            //this.fileSystem = fileSystem;
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

        internal void SendClientMessage(string username, string message)
        {
            foreach (Client client in clients)
            {
                if (client.UserName == username)
                {
                    client.Write(message);
                }
            }
        }
    }
}