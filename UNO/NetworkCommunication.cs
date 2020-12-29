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
using System.Windows;

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
        public LoginViewModel loginViewModel { get; set; }

        //--LobbyRelated--
        public string lobby;
        public List<Score> Scoreboard { get; set; }
        public MainWindowViewModel mainWindowViewModel { get; set; }

  //--Game related--
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
            Debug.WriteLine("Final statement");
            running = false;
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
                    
                    MoveMessage MM = pakket.ToObject<MoveMessage>();
                    bool isvoid = MM.isVoidMove;
                    Card cardmoved = MM.playedCard;
                    if (MM.UserName == user.name)
                    {
                        if (isvoid)
                        {
                            GameScreenViewModel.addCardToUI(cardmoved);
                            GameScreenViewModel.editPlayerCardsInfo(user.name, 1);
                           
                        }
                        else
                        {
                            GameScreenViewModel.removeCardFromUI(cardmoved);
                            GameScreenViewModel.editPlayerCardsInfo(user.name, -1);
                            GameScreenViewModel.changePileCard(cardmoved);
                        }
                    }
                    else
                    {
                        if (!isvoid)
                        {
                            GameScreenViewModel.changePileCard(cardmoved);
                            GameScreenViewModel.editPlayerCardsInfo(MM.UserName,-1);
                            break;
                        }
                        GameScreenViewModel.editPlayerCardsInfo(MM.UserName, 1);
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
                            loginViewModel.MakeMessageBox("Username already in use");
                            isLobbyReady = false;
                            //TODO: show on screen via popup
                            break;
                        case 202:
                            loginViewModel.MakeMessageBox("Lobby full");
                            isLobbyReady = false;
                            //TODO: show on screen via popup
                            break;
                        case 203:
                            loginViewModel.MakeMessageBox("Game is in Session");
                            isLobbyReady = false;
                            break;
                    }
                    break;
                case MessageID.GAME:
                    //TODO: Implement GAME
                    string gamemessage = (string)pakket.GetValue("gameMessage");
                    if (gamemessage == "Win")
                    {
                        string username = (string)pakket.GetValue("Username");
                        if (user.name.Equals(username))
                        {
                            GameScreenViewModel.MakeMessageBox("You Won!");
                        }
                        else
                        {
                            GameScreenViewModel.MakeMessageBox("You lost!");
                        }

                        GameScreenViewModel.gameOver();
                        GameScreenViewModel.quitGame();
                        mainWindowViewModel.resetReady();
                    }
                    else if (gamemessage == "UNO!")
                    {

                        string username = (string)pakket.GetValue("Username");
                        if (user.name.Equals(username))
                        {
                            GameScreenViewModel.MakeMessageBox("UNO!");
                        }
                        else
                        {
                            GameScreenViewModel.MakeMessageBox(username + " Has UNO!");
                        }
                    }
                    else if (gamemessage == "statusUpdate")
                    {
                        //TODO: update left player info
                    }
                    else if (gamemessage == "ToggleReady")
                    {
                        mainWindowViewModel.readyPlayer(messageUsername);
                    }
                    else if(gamemessage == "left Game")
                    {
                        string username = (string) pakket.GetValue("Username");
                        if (!user.name.Equals(username))
                        {
                            mainWindowViewModel.RemovePlayer(username);
                            GameScreenViewModel.RemovePlayer(username);
                        }
                    }

                    break;
                case MessageID.CHAT:
                    //TODO: Implement CHAT
                    //TODO: show chatmessage on chat area
                    ChatMessage message = pakket.ToObject<ChatMessage>();
                    GameScreenViewModel.receiverChatMessage(message);
                    break;
                case MessageID.TURN:
                    //TODO: Implement TURN
                    string name = (string) pakket.GetValue("nextplayer");
                    GameScreenViewModel.changePlayerPlayingName(name);
                    TurnMessage turn = pakket.ToObject<TurnMessage>();
                    Debug.WriteLine(user.name + " and " + name);
                    if (user.name.Equals(name))
                    {Debug.WriteLine("HERE!! ");
                        List<Card> added = turn.addedCards;
                        GameScreenViewModel.AddMultpileCards(added);
                        GameScreenViewModel.setPlayingState(true);
                        GameScreenViewModel.editPlayerCardsInfo(name, added.Count);
                    }
                    else if (name == "System")
                    {
                        GameScreenViewModel.setPlayingState(false);
                        List<Card> pile = turn.addedCards;
                        Card startPileCard = pile[0];
                        Debug.WriteLine("This is the first pilecard:"+startPileCard.SourcePath);
                        GameScreenViewModel.changePileCard(startPileCard);
                        GameScreenViewModel.ResetCardAmounts();
                    }
                    else
                    {
                        GameScreenViewModel.setPlayingState(false);
                        if (turn.addedCards.Count >= 2)
                        {
                            GameScreenViewModel.editPlayerCardsInfo(name, turn.addedCards.Count);
                        }
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


        public void resetToLobby()
        {
           
        }

        public void resetToLogin()
        { 
            lobby = "";
            sendQuitGame();
            mainWindowViewModel.emptyObservableUsers();
        }

        //
        //--Outgoing data--
        //

        public void sendMove(Card playedCard)
        {
            MoveMessage MM = new MoveMessage(playedCard, user.name, false); 
            write(JsonSerializer.Serialize(MM));
        }

        public void sendChat(string message)
        {
            Debug.WriteLine("Chatmessage sendt:" + message);
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
            MoveMessage MS = new MoveMessage(null, user.name,true);
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

        public void sendQuitGame() 
        {
            GameMessage gm = new GameMessage(user.name, "left Game");
            write(JsonSerializer.Serialize(gm));
        }
        
    }
}
