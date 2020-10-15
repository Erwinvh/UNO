using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace Server
{
    class Client
    {
        private TcpClient tcpClient;
        private NetworkStream stream;
        private Server server;
        public string UserName { get; set; }
        public User user;

        public Client(TcpClient tcpClient, Server server)
        {
            this.tcpClient = tcpClient;
            this.server = server;
            Thread listernerThread = new Thread(() => Listener());
            listernerThread.Start();
        }

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
            string id = (string) pakket.GetValue("ID");

            switch (id)
            {
                case "CHAT":
                    //TODO: implement incoming Chat 
                    break;
                case "MOVE":
                    //TODO: implement incoming move
                    break;
            }
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
