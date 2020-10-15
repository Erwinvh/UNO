using System;
using System.Collections.Generic;

namespace SharedDataClasses
{
    public enum MessageID{
        CHAT,
        TURN,
        SYSTEM,
        MOVE,
        GAME
    }


    class TurnMessage
    {
        //This message dictates the turning of the player turn, who was the last player and who is the next. and it also adds the 
        private MessageID MessageID = MessageID.TURN;

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
        private MessageID MessageID = MessageID.MOVE;
        public Card playedCard { get; set; }
        public string UserName { get; set; }

        public MoveMessage(Card played, string byUserName)
        {
            UserName = byUserName;
            playedCard = played;
        }
    }

    class GameMessage
    {
        private MessageID MessageID = MessageID.GAME;

        public string username { get; set; }
        public string gameMessage { get; set; }

        public GameMessage(string username, string gameMessage)
        {
            this.username = username;
            this.gameMessage = gameMessage;
        }
    }
    

    class ChatMessage
    {
        private MessageID MessageID = MessageID.CHAT;
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
        private MessageID MessageID = MessageID.SYSTEM;

        public int status { get; set; }

        public SystemMessage(int status)
        {
            this.status = status;
        }
    }
}
