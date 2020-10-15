using System;
using System.Collections.Generic;
using System.Text;
using SharedDataClasses;

namespace SharedDataClasses
{
    class User
    {
        public List<Card> hand { get; set; }

        public string name { get; set; }
        public bool isHost { get; set; }

        public User(string name)
        {
            this.name = name;
            hand = new List<Card>();
        }

        public void addCardToHand(Card card)
        {
            hand.Add(card);
        }
    }
}
