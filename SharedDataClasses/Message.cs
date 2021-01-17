using System;
using System.Collections.Generic;
using System.Dynamic;

namespace SharedDataClasses
{
    public enum MessageID{
        CHAT,
        TURN,
        SYSTEM,
        MOVE,
        GAME,
        LOBBY,
        SCORE,
        VOID
    }
   
    public class ScoreMessage
    {
        public MessageID MessageID { get; }
        public List<Score> Scores { get; set; }
        public ScoreMessage(List<Score> scores)
        {
            MessageID = MessageID.SCORE;
            Scores = scores;
        }

    }

    public class TurnMessage
    {
        //This message dictates the turning of the player turn, who was the last player and who is the next. and it also adds the 
        public MessageID MessageID { get; }

        public List<Card> addedCards { get; set; }
        public string lastplayer { get; set; }
        public string nextplayer { get; set; }


        public TurnMessage(string lastplayer, string nextplayer, List<Card> addedCards)
        {
            MessageID = MessageID.TURN;
            this.lastplayer = lastplayer;
            this.nextplayer = nextplayer;
            this.addedCards = addedCards;
        }

    }

    public class MoveMessage
    {
        public MessageID MessageID { get; }
        public Card playedCard { get; set; }
        public string UserName { get; set; }
        public bool isVoidMove { get; set; }


        public MoveMessage(Card played, string byUserName, bool isVoid)
        {
            MessageID = MessageID.MOVE;
            UserName = byUserName;
            playedCard = played;
            isVoidMove = isVoid;
        }
    }

    public class GameMessage
    {
        public MessageID MessageID { get; }

        public string Username { get; set; }
        public string gameMessage { get; set; }
        public Dictionary<string, int> playerstatus { get; set; }

        public GameMessage(string username, string gameMessage)
        {
            MessageID = MessageID.GAME;
            this.Username = username;
            this.gameMessage = gameMessage;
        }

        public GameMessage(Dictionary<string, int> statusPlayers)
        {
            MessageID = MessageID.GAME;
            this.playerstatus = statusPlayers;
            this.gameMessage = "statusUpdate";
        }
    }
    

    public class ChatMessage
    {
        public MessageID MessageID { get; }
        public string message { get; set; }
        public string sender { get; set; }
        public DateTime DToS { get; set; }

        public ChatMessage(string sender, string message, DateTime dToS)
        {
            MessageID = MessageID.CHAT;
            this.sender = sender;
            this.message = message;
            DToS = dToS;
        }
    }

    public class SystemMessage
    {
        public MessageID MessageID { get; }

        public int status { get; set; }

        public SystemMessage(int status)
        {
            MessageID = MessageID.SYSTEM;
            this.status = status;
        }
    }

    public class LobbyMessage
    {
        public MessageID MessageID { get; }

        public string Username { get; }
        public string LobbyCode { get; }

        public LobbyMessage(string username, string lobbyCode)
        {
            MessageID = MessageID.LOBBY;
            Username = username;
            LobbyCode = lobbyCode;
        }

    }

}
