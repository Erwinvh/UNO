using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using SharedDataClasses;

namespace SharedDataClasses
{
    class TurnMessage
    {
        //This message dictates the turning of the player turn, who was the last player and who is the next. and it also adds the 
        private string ID = "TURN";

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
        private string ID = "MOVE";
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
        private string ID = "GAME";

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
        private string ID = "CHAT";
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
        private string ID = "SYSTEM";

        public int status { get; set; }

        public SystemMessage(int status)
        {
            this.status = status;
        }
    }
}
