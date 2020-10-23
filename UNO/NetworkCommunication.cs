using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using SharedDataClasses;
using static SharedDataClasses.Encryption;
using JsonSerializer = System.Text.Json.JsonSerializer;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;

namespace UNO
{
    public class NetworkCommunication
    {  
        public User user { get; set; }
        //--Coms-related--
        private TcpClient client;
        private NetworkStream stream;
        public bool running = true;

        //--GUI related--
        public App app { get; set; }
        public bool? isLobbyReady { get; set; }

        //--LobbyRelated--
        private string lobby;
        public List<Score> Scoreboard { get; set; }


  //--Game related--
  public MainWindowViewModel mainWindowViewModel { get; set; }
        public GameScreenViewModel GameScreenViewModel { get; set; }


        public NetworkCommunication(string hostname, int port)
        {
            client = new TcpClient();
            client.BeginConnect(hostname, port, new AsyncCallback(OnConnect), null);
        }

        private void OnConnect(IAsyncResult ar)
        {
            client.EndConnect(ar);
            Debug.WriteLine("Verbonden!");
            stream = client.GetStream();
            Thread listenerThread = new Thread(() => Listener());
            listenerThread.Start();
        }

        public void disconnect()
        {
            SystemMessage sm = new SystemMessage(200);
            write(JsonSerializer.Serialize(sm));
            running = false;
            //stream.Close();
            //client.Close();
        }

        public void updateUI()
        {
            //TODO: implement method
        }


        //
        //--Incoming data--
        //

        public void Listener()
        {
            Byte[] lengtebytes = new byte[2];
            while (running)
            {
                for (int i = 0; i < 2; i++)
                {
                    lengtebytes[i] = (byte)stream.ReadByte();
                }
                uint bytes = BitConverter.ToUInt16(lengtebytes);
                //bytes += 2;
                Byte[] bytebuffer = new byte[bytes];
                for (int i = -2; i < bytes; i++)
                {
                    if (i >= 0)
                    {
                        bytebuffer[i] = (byte)stream.ReadByte();
                    }
                    else
                    {
                        byte Byte = (byte)stream.ReadByte();
                    }


                }
                string listenedData = Encoding.ASCII.GetString(bytebuffer);
                //listenedData.Remove(0, 2);
                Debug.WriteLine(listenedData);
                handleData(listenedData);
            }
            stream.Close();
            client.Close();
        }

        private void handleData(string packetData)
        {
            Debug.WriteLine($"Got a packet: {packetData}");
            JObject pakket;
            MessageID messageId;
            string messageUsername;

            try
            {
                pakket = JObject.Parse(packetData);
                Enum.TryParse((string)pakket.GetValue("MessageID"), out messageId);
                messageUsername = (string)pakket.GetValue("Username");
            } catch (Exception e)
            {
                pakket = null;
                messageId = MessageID.VOID;
                messageUsername = "";
            }

            switch (messageId)
            {
                case MessageID.MOVE:
                    bool isvoid = (bool)pakket.GetValue("isVoidMove");
                    Card cardmoved = JsonSerializer.Deserialize<Card>((string)pakket.GetValue("playedCard"));
                    if (messageUsername == user.name)
                    {
                        if (!isvoid)
                        {
                            GameScreenViewModel.removeCardFromUI(cardmoved);
                        }
                        else
                        {
                            GameScreenViewModel.addCardToUI(cardmoved);
                        }
                    }
                    else
                    {
                        if (!isvoid)
                        {
                            GameScreenViewModel.changePileCard(cardmoved);
                        }
                    }
                    break;
                case MessageID.SYSTEM:
                    //TODO: Implement SYSTEM
                    int code = (int)pakket.GetValue("status");
                    Debug.WriteLine(code);
                    switch (code)
                    {
                        case 101:
                            Debug.WriteLine("Username OK");
                            break;
                        case 102:
                            Debug.WriteLine("Lobby OK");
                            isLobbyReady = true;
                            Thread.Sleep(30);
                            mainWindowViewModel.AddPlayer(user.name);
                            break;
                        case 201:
                            Console.WriteLine("Username already in use");
                            isLobbyReady = false;
                            //TODO: show on screen via popup
                            break;
                        case 202:
                            Console.WriteLine("Lobby full");
                            isLobbyReady = false;
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
                        GameScreenViewModel.EmptyHand();
                        //TODO: return to the lobby
                    }
                    else if (gamemessage == "lose")
                    {
                        //TODO: show lose message
                        GameScreenViewModel.EmptyHand();
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
                    else if (gamemessage == "ToggleReady")
                    {
                        getUserObsColl(messageUsername).isReady = !getUserObsColl(messageUsername).isReady;
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
                        TurnMessage turn = pakket.ToObject<TurnMessage>();
                        List<Card> added = turn.addedCards;
                        GameScreenViewModel.AddMultpileCards(added);
                        GameScreenViewModel.setPlayingState(true);
                    }
                    else if (user.name == (string)pakket.GetValue("lastPlayer"))
                    {
                        GameScreenViewModel.setPlayingState(false);
                    }
                    break;
                case MessageID.LOBBY:
                    string lobbyCode = (string)pakket.GetValue("LobbyCode");
                    if (lobbyCode != lobby)
                    {
                        Debug.WriteLine("Remove player");
                        mainWindowViewModel.RemovePlayer(messageUsername);
                    }
                    else
                    {
                        Debug.WriteLine("OLd player added to new player");
                        mainWindowViewModel.AddPlayer(messageUsername);
                    }
                    break;
                case MessageID.SCORE:
                    ScoreMessage score = pakket.ToObject<ScoreMessage>();
                    Scoreboard = score.Scores;
                    break;
            }
        }

        public User getUserObsColl(string username)
        {
            foreach (User player in mainWindowViewModel.observableUsers)
            {
                if (player.name == username)
                {
                    return player;
                }
            }
            return null;
        }


        //
        //--Outgoing data--
        //

        public void sendMove(Card playedCard)
        {
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
            isLobbyReady = null;
            // check thisV
            user = new User(Username);
            lobby = LobbyCode;
            if (lobby == "")
            {
                mainWindowViewModel.emptyObservableUsers();
                //isLobbyReady = null;
            }
            LobbyMessage LM = new LobbyMessage(user.name, LobbyCode);
            write(JsonSerializer.Serialize(LM));
        }

        public void sendToggleReady()
        {
            GameMessage GM = new GameMessage(user.name, "ToggleReady");
            write(JsonSerializer.Serialize(GM));
        }

        public void sendEmptyMove()
        {
            MoveMessage MS = new MoveMessage(null, user.name);
            write(JsonSerializer.Serialize(MS));
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
