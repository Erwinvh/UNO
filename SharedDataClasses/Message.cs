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
        LOBBY
    }


    class TurnMessage
    {
        //This message dictates the turning of the player turn, who was the last player and who is the next. and it also adds the 
        public MessageID MessageID = MessageID.TURN;

        public List<Card> addedCards { get; set; }
        public string lastplayer { get; set; }
        public string nextplayer { get; set; }


        public TurnMessage(string lastplayer, string nextplayer, List<Card> addedCards)
        {
            this.lastplayer = lastplayer;
            this.nextplayer = nextplayer;
            this.addedCards = addedCards;
        }

    }

    class MoveMessage
    {
        public MessageID MessageID = MessageID.MOVE;
        public Card playedCard { get; set; }
        public string UserName { get; set; }
        public bool isVoidMove { get; set; }

        public MoveMessage(Card played, string byUserName)
        {
            UserName = byUserName;
            playedCard = played;
            isVoidMove = false;
        }
        public MoveMessage(Card played, string byUserName, bool isVoid)
        {
            UserName = byUserName;
            playedCard = played;
            isVoidMove = isVoid;
        }
    }

    class GameMessage
    {
        public MessageID MessageID = MessageID.GAME;

        public string Username { get; set; }
        public string gameMessage { get; set; }
        public Dictionary<string, int> playerstatus { get; set; }

        public GameMessage(string username, string gameMessage)
        {
            this.Username = username;
            this.gameMessage = gameMessage;
        }

        public GameMessage(Dictionary<string, int> statusPlayers)
        {
            this.playerstatus = statusPlayers;
            this.gameMessage = "statusUpdate";
        }
    }
    

    class ChatMessage
    {
        public MessageID MessageID = MessageID.CHAT;
        public string message { get; set; }
        public string sender { get; set; }
        public DateTime DToS { get; set; }

        public ChatMessage(string sender, string message, DateTime dToS)
        {
            this.sender = sender;
            this.message = message;
            DToS = dToS;
        }
    }

    class SystemMessage
    {
        public MessageID MessageID = MessageID.SYSTEM;

        public int status { get; set; }

        public SystemMessage(int status)
        {
            this.status = status;
        }
    }

    class LobbyMessage
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
