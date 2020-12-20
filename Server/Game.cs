using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;
using SharedDataClasses;
using JsonSerializer = System.Text.Json.JsonSerializer;

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
        private Lobby lobby { get; set; }

        private const int SkipTurn = 10;
        private const int TurnAround = 11;
        private const int Plus2 = 12;
        private const int Wild = 13;
        private const int Plus4 = 14;

        private Server server { get; }

        public Game(Server server, Lobby lobby)
        {
            this.lobby = lobby;
            players = lobby.players;
            Console.WriteLine("Game has begun");
            deck = new List<Card>();
            pile = new List<Card>();
            this.server = server;
            fillDeck();
            for (int i = 0; i < 5; i++)
            {
                Shuffle();
            }
            lastPlayedCard = deck[deck.Count - 1];
            pile.Add(lastPlayedCard);
            deck.Remove(lastPlayedCard);
            beginGame();
            Shuffle();

            Shuffle();
            firstTurn();
        }

        internal void playerQuitCase(string name)
        {
            User _player = null;
            foreach (User player in players) {
                if (player.name.Equals(name)) {
                    _player = player;
                }
            }

            if (_player != null)
            {
                players.Remove(_player);

                if (_player.name.Equals(name))
                {

                    List<Card> addedCards = new List<Card>();
                    if (isClockwise) 
                    {
                        if (index >= players.Count) 
                        {
                            index = 0;
                        }
                        TurnMessage turn = new TurnMessage(name, players[index].name, addedCards);
                        server.getClient(name).Broadcast(JsonSerializer.Serialize(turn));

                    } else
                    {
                        nextTurn();
                        TurnMessage turn = new TurnMessage(name, players[index].name, addedCards);
                        server.getClient(name).Broadcast(JsonSerializer.Serialize(turn));
                    }                    
                }
            }

            if (players.Count == 1) 
            {
                win(players[0].name);
            } else if(players.Count == 0)
            {
                //TODO Close game, no winner
            }

            List<Card> hand = server.getClient(name).hand;

            deck.AddRange(hand);
            Shuffle();
            Shuffle();
            Shuffle();

            server.getClient(name).hand.Clear();
            
        }

        public void win(string name)
        {
            //TODO player wins game
        }

        public void fillDeck()
        {
            for (int i = 0; i < 13; i++)
            {
                for (int j = 0; j < 2; j++)
                { 
                deck.Add(new Card(Card.Color.YELLOW, i));
                deck.Add(new Card(Card.Color.GREEN,i));
                deck.Add(new Card(Card.Color.BLUE,i));
                deck.Add(new Card(Card.Color.RED,i));
                }
            }
            //TODO: implement check black cards
         //  for (int i = 0; i < 4; i++)
         //  {
         //      deck.Add(new Card(Card.Color.BLACK, 13));
         //      deck.Add(new Card(Card.Color.BLACK, 14));
         //  }
        }

        public void beginGame()
        {            
            foreach (User player in players)
            {
                List<Card> addedCards = new List<Card>();
                for (int i = 0; i < 7; i++)
                {
                   addedCards.Add(drawCard(player.name));
                }

                server.getClient(player.name).hand = addedCards;
                TurnMessage initialTurn = new TurnMessage("System", player.name, addedCards);
                server.SendClientMessage(player.name, JsonSerializer.Serialize(initialTurn));
                List<Card> pileCard = new List<Card>();
                pileCard.Add(lastPlayedCard);
                Console.WriteLine("This card is tyhe first pile card: "+lastPlayedCard.SourcePath);
                TurnMessage nullifierTurn = new TurnMessage(player.name, "System", pileCard);
                server.SendClientMessage(player.name, JsonSerializer.Serialize(nullifierTurn));
                
            }
        }

        public GameMessage GeneratePlayerStatusMessage()
        {
            Dictionary<string, int> statusses = new Dictionary<string, int>();
            foreach (User user in players)
            {
                statusses.Add(user.name, server.getClient(user.name).hand.Count);
            }
            GameMessage GM = new GameMessage(statusses);
            return GM;
        }

        public void firstTurn()
        {
            TurnMessage firstTurn = new TurnMessage("System", players[0].name, new List<Card>());
            lobby.sendToAll(JsonSerializer.Serialize(firstTurn));
        }

        public bool compareHandToCard(List<Card> hand, Card card)
        {
            foreach (Card ownedCard in hand)
            {
                if (ownedCard.number == card.number)
                {
                    if (ownedCard.color == Card.Color.BLACK || ownedCard.color == card.color)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool checkMove(Card playedCard, string name)
        {
            if (playedCard==null)
            {
                return false;
            }
            if (playedCard.color==lastPlayedCard.color||playedCard.number==lastPlayedCard.number)
            {
                Console.WriteLine("Check move check message WE ARE HERE!!!" + server.getClient(name).hand.Count);
                
                if (!compareHandToCard(server.getClient(name).hand,playedCard))
                {
                     return false;
                }
                server.getClient(players[index].name).hand.Remove(playedCard);
                lastPlayedCard = playedCard;
                if (playedCard.number==Wild||playedCard.number==Plus4)
                {
                    playedCard.setColor(Card.Color.BLACK);
                }
                pile.Add(playedCard);
                return true;
            } 
            return false;
        }

        public Card drawCard(string player)
        {
            DeckCheck();
            Card drawedCard = deck[deck.Count - 1];
            deck.Remove(drawedCard);
            Console.WriteLine("Hand:"+server.getClient(player).hand);
            server.getClient(player).hand.Add(drawedCard);
            Shuffle();
            return drawedCard;
        }

        private int findIndexofPLayer(string player)
        {
            foreach (User gameplayer in players)
            {
                if (gameplayer.name==player)
                {
                    return players.IndexOf(gameplayer);
                }
            }

            return -1;
        }

        public List<Card> ProcessEffect()
        {
            List<Card> addCards = new List<Card>();
            switch (lastPlayedCard.number)
            {
                case SkipTurn:
                    if (isClockwise)
                    {
                        index++;
                        return null;
                    }
                    index--;
                    return null;
                case TurnAround:
                    isClockwise = !isClockwise;
                    return null;
                case Plus2:
                    if (isClockwise)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            int person = index + 1;
                            if (person >= players.Count)
                            {
                                person = 0;
                            }
                            addCards.Add(drawCard(players[person].name));
                        }
                        return addCards;
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        int person = index - 1;
                        if (person < 0)
                        {
                            person = players.Count - 1;
                        }
                        addCards.Add(drawCard(players[person].name));
                    }
                    return addCards;
                case Plus4:
                    if (isClockwise)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            addCards.Add(drawCard(players[index + 1].name));
                        }
                        return addCards;
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        addCards.Add(drawCard(players[index - 1].name));
                    }
                    return addCards;
            }
            return null;
        }

        internal bool Checkhand()
        {
            if (server.getClient(players[index].name).hand.Count==0)
            {
                //TODO: have server finish other things, dont know what right now
                return true;
            }
            return false;
        }

        internal bool checkUNO()
        {
            if (server.getClient(players[index].name).hand.Count == 1)
            {
                return true;
            }
            return false;
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

  internal TurnMessage GenerateTurn(bool isForfeitTurn)
  {
      string lastplayer = players[index].name;
      List<Card> addedCards = new List<Card>();
      if (!isForfeitTurn)
      {
          List<Card> newCards = ProcessEffect();
          if (newCards!=null)
          {
              addedCards = newCards;
          }
      }
       
            nextTurn();
            TurnMessage turn = new TurnMessage(lastplayer,players[index].name, addedCards);
            return turn;
  }

        public void nextTurn()
        {
            if (isClockwise)
            {
                index++;
                if (index >= players.Count)
                {
                    index = 0;
                }
            }
            else
            {
                index--;
                if (index<0)
                {
                    index = players.Count - 1;
                }
            }
        }
    }
}
