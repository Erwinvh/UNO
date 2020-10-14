using System;
using System.Collections.Generic;
using System.Text;
using SharedDataClasses;

namespace Server
{
    class TurnMessage
    {
        private string ID = "Turn";

        public List<Card> addedCards { get; set; }
        public string lastplayer { get; set; }
        public string nextplayer { get; set; }
        public int lastplayerCardAmount { get; set; }

        public TurnMessage(string lastplayer, int lastplayerCardAmount, string nextplayer, List<Card> addedCards)
        {
            this.lastplayer = lastplayer;
            this.lastplayerCardAmount = lastplayerCardAmount;
            this.nextplayer = nextplayer;
            this.addedCards = addedCards;
        }

    }

    class GameMessage
    {
        private string ID = "GAME";
        public string systemMessage { get; set; }

        public GameMessage(string systemMessage)
        {
            this.systemMessage = systemMessage;
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

}
