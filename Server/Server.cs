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

        //The disconnect method for the clients
        internal void Disconnect(Client client)
        {
            clients.Remove(client);
            Console.WriteLine("Client disconnected");
        }

        //TODO: check if this is usefull
        //this method allows the server to send to one specific client
        internal void SendToUser(string user, string packet)
        {
            foreach (var client in clients.Where(c => c.UserName == user))
            {
                client.Write(packet);
            }
        }



        public bool checkClientsForUser(string username)
        {
            foreach (Client client in clients)
            {
                if (client.UserName == username)
                {
                    return true;
                }
            }
            return false;
        }

        public Client getClientByUser(string username)
        {
            foreach (Client client in clients)
            {
                if (client.UserName == username)
                {
                    return client;
                }
            }
            return null;
        }

        public void Broadcast()
        {

        }

    }



}