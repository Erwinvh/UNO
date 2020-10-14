using System;
using System.Collections.Generic;
using System.Text;
using SharedDataClasses;


namespace Server
{
    class Game
    {
        private List<User> players;
        private bool isClockwise = true;
        private int index = 0;
        private List<Card> deck { get; set; }
        private List<Card> pile { get; set; }
        private Card lastPlayedCard { get; set; }
        private bool isOngoing = true;

        private const int SkipTurn = 10;
        private const int TurnAround = 11;
        private const int Plus2 = 12;
        private const int Wild = 13;
        private const int Plus4 = 14;

        public Game()
        {
            //new game starts
            //set server in playing mode
            //foreach loop give each player 7 cards.
            while (isOngoing)
            {
                //game is played here
            }
            //finish game
        }

        public void checkMove(Card playedCard)
        {
            if (playedCard==null)
            {
                //players[index].hand.add(randomCard());
            }
            else
            {
                //TODO: check if played card have same number or color as lastPlayedCard
                //TODO: check current players hadn if he has 1 or 0 card left.
            }
            //TODO: send current player that played confirm
            //TODO: broadcast current player amount of cards
            //TODO: Go to proccess effect
        }

        public void ProcessEffect()
        {
            //TODO: Process the effect of the lastplayed card for the next user
            switch (lastPlayedCard.number)
            {
                case SkipTurn:
                    if (isClockwise)
                    {
                        index++;
                        break;
                    }
                    index--;
                        break;
                case TurnAround:
                    isClockwise = !isClockwise;
                    break;
                case Plus2:
                    //TODO: add 2 to next player
                    break;
                case Wild:
                    //TODO: change the color to currents player chosen color.
                    break;
                case Plus4:
                    //TODO: add 4 cards to next player
                    break;
            }
            //TODO: next turn
        }

        public void DeckCheck()
        {
            if (deck.Count==0)
            {
                foreach (Card card in pile)
                {
                    if (card != lastPlayedCard)
                    {
                        deck.Add(card);
                    }
                }
                pile.Clear();
                pile.Add(lastPlayedCard);
                Shuffle();
            }
        }

        public void Shuffle()
        {
            Random RN = new Random();
            int n = deck.Count;
            while (n > 1)
            {
                n--;
                int k = RN.Next(n + 1);
                Card value = deck[k];
                deck[k] = deck[n];
                deck[n] = value;
            }
        }

        public void nextTurn()
        {


            if (isClockwise)
            {
                index++;
            }
            else
            {
index--;
            }
            //let user play
        }


    }
}
