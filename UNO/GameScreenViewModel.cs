using SharedDataClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace UNO
{
    public class GameScreenViewModel
    {
        public ObservableCollection<Card> hand { get; set; }

        public GameScreenViewModel(App app, NetworkCommunication networkCommunication)
        {
            hand = new ObservableCollection<Card>();
            hand.Add(new Card(Card.Color.GREEN, 1));
            hand.Add(new Card(Card.Color.GREEN, 2));
            hand.Add(new Card(Card.Color.GREEN, 3));
            hand.Add(new Card(Card.Color.GREEN, 4));
            hand.Add(new Card(Card.Color.GREEN, 5));
            hand.Add(new Card(Card.Color.GREEN, 6));
            hand.Add(new Card(Card.Color.GREEN, 7));
            hand.Add(new Card(Card.Color.GREEN, 8));
            hand.Add(new Card(Card.Color.GREEN, 9));
            hand.Add(new Card(Card.Color.GREEN, 0));
            hand.Add(new Card(Card.Color.GREEN, 1));
            hand.Add(new Card(Card.Color.GREEN, 2));
            hand.Add(new Card(Card.Color.GREEN, 3));
            hand.Add(new Card(Card.Color.GREEN, 4));
            hand.Add(new Card(Card.Color.GREEN, 5));
        } 

        
    }
}
