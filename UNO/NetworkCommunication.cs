using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SharedDataClasses;
using static SharedDataClasses.Encryption;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace UNO
{
    public class NetworkCommunication
    {
        private TcpClient client;
        private User user;
        private NetworkStream stream;

        public NetworkCommunication(string hostname, int port)
        {
            client = new TcpClient();
            user = new User(hostname);
            client.BeginConnect(hostname, port, new AsyncCallback(OnConnect), null);
        }



        private void OnConnect(IAsyncResult ar)
        {
            client.EndConnect(ar);
            Console.WriteLine("Verbonden!");
            stream = client.GetStream();
            Thread listenerThread = new Thread(() => Listener());
            listenerThread.Start();
        }
        public void disconnect()
        {

        }

        //
        //--Incoming data--
        //

        public Task Listener()
        {
            Byte[] lengtebytes = new byte[2];
            while (true)
            {
                for (int i = 0; i < 2; i++)
                {
                    lengtebytes[i] = (byte)stream.ReadByte();
                }
                uint bytes = BitConverter.ToUInt16(lengtebytes);
                Byte[] bytebuffer = new byte[bytes];
                for (int i = -3; i < bytes; i++)
                {
                    bytebuffer[i] = (byte)stream.ReadByte();
                }
                string rawOutput = Encoding.ASCII.GetString(bytebuffer);

                Console.WriteLine(rawOutput);
                handleData(Decode(rawOutput));
            }
        }

        private void handleData(string packetData)
        {
            Console.WriteLine($"Got a packet: {packetData}");
            JObject pakket = JObject.Parse(packetData);
            string id = (string)pakket.GetValue("ID");
            string messageUsername = (string) pakket.GetValue("Username");
            switch (id)
            {
                case "MOVE":
                    if (messageUsername == user.name)
                    {
                        Card cardmoved = JsonSerializer.Deserialize<Card>((string) pakket.GetValue("playedCard"));
                        if (user.hand.Contains((cardmoved)))
                        {
                            user.hand.Remove(cardmoved);
                        }
                        else
                        {
                            user.hand.Add(cardmoved);
                        }
                    }
                    break;
                case "SYSTEM":
                    //TODO: Implement SYSTEM
                    break;
                case "GAME":
                    //TODO: Implement GAME
                    string gamemessage = (string) pakket.GetValue("gameMessage");
                    if (gamemessage=="Win")
                    {
                        //TODO: show win message
                        //TODO: return to the lobby
                    }
                    else if (gamemessage == "lose")
                    {
                        //TODO: show lose message
                        //TODO: return to the lobby
                    }
               

                    break;
                case "CHAT":
                    //TODO: Implement CHAT
                    //TODO: show chatmessage on chat area

                    break;
                case "TURN":
                    //TODO: Implement TURN
                    if (user.name == (string)pakket.GetValue("nextPlayer"))
                    {                        
                        //TODO: convert jobject to List<Card>
                        List<Card> added = new List<Card>();
                        user.hand.AddRange(added);
                        //TODO: set player ui to playing

                    }
                    else if (user.name == (string)pakket.GetValue("lastPlayer"))
                    {
                        //TODO: set player ui to NOT playing
                    }
                    break;
            }
        }


        //
        //--Outgoing data--
        //

        public void sendMove(Card playedCard, Card.Color color)
        {
            if (playedCard.number == 13||playedCard.number == 14)
            {
                playedCard.color = color;
            }
            MoveMessage MM = new MoveMessage(playedCard, user.name); 
            write(JsonSerializer.Serialize(MM));
        }

        public void sendChat(string message)
        {
            ChatMessage CM = new ChatMessage(user.name, message, DateTime.Now);
            write(JsonSerializer.Serialize(CM));
        }

        public void write(string data)
        {
            string encodedData = Encode(data);
            int length = encodedData.Length;
            Console.WriteLine(data);
            Byte[] lengteBytes = BitConverter.GetBytes(length);
            var dataAsBytes = Encoding.ASCII.GetBytes(encodedData);
            Byte[] z = lengteBytes.Concat(dataAsBytes).ToArray();
            stream.Write(z, 0, z.Length);
            stream.Flush();
        }
        
    }
}
