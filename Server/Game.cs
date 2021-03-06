﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;
using SharedDataClasses;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Server
{
     public class Game
     {
        private List<User> players;
        private bool isClockwise = false;
        private bool needseffect = true;
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

        //
        //--Handles player quitting the game--
        //
        internal void playerQuitCase(string name)
        {
            int _player = -1;

            foreach (User player in players) {
                if (player.name.Equals(name)) {
                    _player = players.IndexOf(player);
                }
            }

            if (_player != -1)
            {
                if (players[_player].name.Equals(name))
                {
                    Console.WriteLine("LOSER");
                    List<Card> addedCards = new List<Card>();
                    if (isClockwise)
                    {
                        int indexCounter = index;
                        indexCounter++;
                        if (indexCounter >= players.Count)
                        {
                            indexCounter = 0;
                        }
                        Console.WriteLine(players[indexCounter].name);

                        foreach (User player in players)
                        {
                            TurnMessage turn = new TurnMessage(name, players[indexCounter].name, addedCards);
                            server.SendClientMessage(player.name, JsonSerializer.Serialize(turn));
                        }
                    }
                    else
                    {
                        nextTurn();
                        foreach (User player in players)
                        {
                            TurnMessage turn = new TurnMessage(name, players[index].name, addedCards);
                            server.SendClientMessage(player.name, JsonSerializer.Serialize(turn));
                        }
                    }
                }
                players.Remove(players[_player]);
                if (!isClockwise && index >= players.Count)
                {
                    index = players.Count - 1;
                }
            }

            if (players.Count == 1) 
            {
                win(players[0].name);
                server.fileSystem.WritetoFile();
            } 
            else if(players.Count == 0)
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

        //
        //--Handles player winning a game--
        //
        public void win(string name)
        {
            server.fileSystem.getScoreByUser(name).increaseWinAmount();
            server.fileSystem.updateScore(server.fileSystem.getScoreByUser(name));

            lobby.resetReady();

            foreach (User player in players)
            {
                GameMessage win = new GameMessage(name, "Win");
                server.SendClientMessage(player.name, JsonSerializer.Serialize(win));

                server.fileSystem.getScoreByUser(player.name).increaseGameAmount();
                server.fileSystem.updateScore(server.fileSystem.getScoreByUser(player.name));

                Console.WriteLine(player.name + "got sent A win message!");
            }

            foreach (User player in players)
            {
                ScoreMessage updatedScore = new ScoreMessage(server.fileSystem.scoreBoard.scoreboard);
                server.SendClientMessage(player.name, JsonSerializer.Serialize(updatedScore));
            }
        }

        //
        //--Fills the games card deck with the correct cards--
        //
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
            for (int i = 0; i < 4; i++)
            {
                deck.Add(new Card(Card.Color.BLACK, 13));
                deck.Add(new Card(Card.Color.BLACK, 14));
            }
        }

        //
        //--Handles the event of a game beginning--
        //
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

        //
        //--Generates a gamemessage for the status of a player--
        //
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

        //
        //--Used for the first turn in the game--
        //
        public void firstTurn()
        {
            TurnMessage firstTurn = new TurnMessage("System", players[0].name, new List<Card>());
            lobby.sendToAll(JsonSerializer.Serialize(firstTurn));
        }

        //
        //--Compares cards for possible turns--
        //
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

        //
        //--Checks if a player's move is valid--
        //
        public bool checkMove(Card playedCard, string name)
        {
            if (playedCard==null)
            {
                return false;
            }
            if (playedCard.color == lastPlayedCard.color || 
                playedCard.number == lastPlayedCard.number || 
                playedCard.number == Wild || 
                playedCard.number == Plus4 || 
                lastPlayedCard.color == Card.Color.BLACK
                ) {
                if (!compareHandToCard(server.getClient(name).hand, playedCard))
                {
                    return false;
                }
                server.getClient(name).RemoveCard(playedCard.number, playedCard.color);
                lastPlayedCard = playedCard;
                needseffect = true;
                
                pile.Add(playedCard);
                return true;
            }
            return false;
        }

        //
        //--Handles drawing a card from the deck when a player is not able to play a valid moive--
        //
        public Card drawCard(string player)
        {
            DeckCheck();
            Card drawedCard = deck[deck.Count - 1];
            deck.Remove(drawedCard);
            Console.WriteLine("Hand:" + server.getClient(player).hand);
            server.getClient(player).hand.Add(drawedCard);
            Shuffle();
            return drawedCard;
        }

        //
        //--Handles the effects of special cards eg. plus 2/4 and skip turn--
        //
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
                            int person = index + 1;
                            if (person >= players.Count)
                            {
                                person = 0;
                            }
                            addCards.Add(drawCard(players[person].name));
                        }
                        return addCards;
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        int person = index - 1;
                        if (person < 0)
                        {
                            person = players.Count - 1;
                        }
                        addCards.Add(drawCard(players[person].name));
                    }
                    return addCards;
            }
            return null;
        }

        //
        //--Checks a player's hand for card count--
        //
        internal bool Checkhand()
        {
            if (server.getClient(players[index].name).hand.Count==0)
            {
                
                return true;
            }
            return false;
        }

        //
        //--Checks if a player has UNO(1 card left in hand)--
        //
        internal bool checkUNO()
        {
            if (server.getClient(players[index].name).hand.Count == 1)
            {
                return true;
            }
            return false;
        }

        //
        //--Checks the amount of cards left in the deck. If deck is empty it will shuffle the pile
        //  and put the played cards of the pile back in the deck--
        //
        public void DeckCheck()
        {
            if (deck.Count==0)
            {
                foreach (Card card in pile)
                {
                    if (card != lastPlayedCard)
                    {
                        if (card.number == 13 || card.number == 14)
                        {
                            card.setColor(Card.Color.BLACK);
                        }
                        deck.Add(card);
                    }
                }
                pile.Clear();
                pile.Add(lastPlayedCard);
                Shuffle();
            }
        }

        //
        //--Shuffles the deck--
        //
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

        //
        //--Processes a turn--
        //
        internal TurnMessage GenerateTurn(bool isForfeitTurn)
        {
            string lastplayer = players[index].name;
            List<Card> addedCards = new List<Card>();
            if (!isForfeitTurn && needseffect)
            {
                List<Card> newCards = ProcessEffect();
                if (newCards!=null)
                {
                    addedCards = newCards;
                }
                needseffect = false;
            }
            nextTurn();
            TurnMessage turn = new TurnMessage(lastplayer,players[index].name, addedCards);
            return turn;
        }

        //
        //--Handles the games next turn, checks whether the game is going clockwise or anti-clokcwise--
        //
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
