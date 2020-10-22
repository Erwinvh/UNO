using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using SharedDataClasses;
using static SharedDataClasses.Encryption;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace UNO
{
    public class NetworkCommunication
    {
        //--Coms-related--
        private TcpClient client;
        private NetworkStream stream;


        //--Game related--
        private User user;
        private Card pileCard;
        private bool isplaying;
        private string lobby;
        private Dictionary<string, int> playerDictionary { get; set; }

        public NetworkCommunication(string hostname, int port)
        {
            client = new TcpClient();
            
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
            //TODO: Implement method
        }

        public void updateUI()
        {
            //TODO: implement method
        }

        public void updateLobbyUI()
        {
            //TODO: implement method
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

        private void handleData(string packetData)
        {
            Console.WriteLine($"Got a packet: {packetData}");
            JObject pakket = JObject.Parse(packetData);
            MessageID messageId;
            Enum.TryParse((string)pakket.GetValue("MessageID"), out messageId);
            string messageUsername = (string)pakket.GetValue("Username");
            switch (messageId)
            {
                case MessageID.MOVE:
                    bool isvoid = (bool)pakket.GetValue("isVoidMove");
                    Card cardmoved = JsonSerializer.Deserialize<Card>((string)pakket.GetValue("playedCard"));
                    if (messageUsername == user.name)
                    {
                        if (!isvoid)
                        {
                            user.hand.Remove(cardmoved);
                            updateUI();
                        }
                        else
                        {
                            user.hand.Add(cardmoved);
                            updateUI();
                        }
                    }
                    else
                    {
                        if (!isvoid)
                        {
                            pileCard = cardmoved;
                            updateUI();
                        }
                    }
                    break;
                case MessageID.SYSTEM:
                    //TODO: Implement SYSTEM
                    int code = (int)pakket.GetValue("status");
                    switch (code)
                    {
                        case 101:
                            Console.WriteLine("Username OK");

                            break;
                        case 102:
                            Console.WriteLine("Lobby OK");
                            playerDictionary = new Dictionary<string, int>();
                            //TODO: send user to lobbyscreen
                            break;
                        case 201:
                            Console.WriteLine("Username already in use");
                            //TODO: show on screen via popup
                            break;
                        case 202:
                            Console.WriteLine("Lobby full");
                            //TODO: show on screen via popup
                            break;
                    }
                    break;
                case MessageID.GAME:
                    //TODO: Implement GAME
                    string gamemessage = (string)pakket.GetValue("gameMessage");
                    if (gamemessage == "Win")
                    {
                        //TODO: show win message
                        user.hand = new List<Card>();
                        //TODO: return to the lobby
                    }
                    else if (gamemessage == "lose")
                    {
                        //TODO: show lose message
                        user.hand = new List<Card>();
                        //TODO: return to the lobby
                    }
                    else if (gamemessage == "UNO!")
                    {
                        //TODO: display onto screen who has uno
                    }
                    else if (gamemessage == "statusUpdate")
                    {
                        //TODO: update left player info
                    }

                    break;
                case MessageID.CHAT:
                    //TODO: Implement CHAT
                    //TODO: show chatmessage on chat area

                    break;
                case MessageID.TURN:
                    //TODO: Implement TURN
                    if (user.name == (string)pakket.GetValue("nextPlayer"))
                    {
                        List<Card> added = JsonSerializer.Deserialize<List<Card>>((string)pakket.GetValue("addedCards"));
                        user.hand.AddRange(added);
                        isplaying = true;
                        updateUI();
                    }
                    else if (user.name == (string)pakket.GetValue("lastPlayer"))
                    {
                        isplaying = false;
                        updateUI();
                    }
                    break;
                case MessageID.LOBBY:
                    string lobbyCode = (string)pakket.GetValue("LobbyCode");
                    if (lobbyCode == "" || lobbyCode != lobby)
                    {
                        playerDictionary.Remove(messageUsername);
                        updateLobbyUI();

                    }
                    else
                    {
                        playerDictionary.Add(messageUsername, -1);
                        updateLobbyUI();
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

        public void sendLobby(string Username, string LobbyCode)
        {
            user = new User(Username);
            lobby = LobbyCode;
            LobbyMessage LM = new LobbyMessage(user.name, LobbyCode);
            write(JsonSerializer.Serialize(LM));
        }

        public void write(string data)
        {
            string encodedData = Encode(data);
            //int length = encodedData.Length;
            int length = data.Length;
            Console.WriteLine(data);
            Byte[] lengteBytes = BitConverter.GetBytes(length);
            var dataAsBytes = Encoding.ASCII.GetBytes(data);
            Byte[] z = lengteBytes.Concat(dataAsBytes).ToArray();
            stream.Write(z, 0, z.Length);
            stream.Flush();
        }
        
    }
}
