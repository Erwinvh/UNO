using SharedDataClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using System.Diagnostics;
using System.Windows.Media;

namespace UNO
{
    public class GameScreenViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Card> hand { get; set; }
        public ICommand ChatCommand { get; set; }
        public ICommand DeckCommand { get; set; }
        public ICommand MoveCommand { get; set; }
        readonly App app;
        private NetworkCommunication networkCommunication;
        public string imageSource { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public string Message { set; get; }


        public GameScreenViewModel(App app, NetworkCommunication networkCommunication)
        {
            this.networkCommunication = networkCommunication;
            //this.LeaveLobbyCommand = new RelayCommand(() => { LeaveLobby(); }); 
            //this.LaunchGameCommand = new RelayCommand(() => { LaunchGame(); }); 
            this.app = app;
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
            ChatCommand = new RelayCommand(() => { sendChatmessage(Message); });
            DeckCommand = new RelayCommand(() => { pullFromDeck(); });
            MoveCommand = new RelayCommand<Card>(sendMove);
        }

        // 
        //--UI to Netrwerkcom-- 
        // 
        public void pullFromDeck()
        {
            networkCommunication.sendEmptyMove();
        }

        public void sendChatmessage(string message)
        {
            networkCommunication.sendChat(message);
        }

        public void sendMove(string source)
        {
            Card movedCard = null;
            foreach (Card card in hand)
            {
                if (card.SourcePath == source)
                {
                    movedCard = card;
                }
            }
            //TODO: add wildcard logic
           // if (playedCard.number == 13 || playedCard.number == 14)
           // {
            //    playedCard.color = color;
            //}
            networkCommunication.sendMove(movedCard); 
        }


        // 
        //--netwerkcom to UI-- 
        // 
        public void addCardToUI(Card card)
        {
            //TODO: add card to UI 
        }

        public void removeCardFromUI(Card card)
        {
            //TODO: try via index or via card 
        }

        public void changePileCard(Card card)
        {
            //TODO: change pileCard 
        }

        public void setPlayingState(bool isPlaying)
        {
            if (isPlaying)
            {
                //TODO: set buttons and other elements but chat to pressable 
                return;
            }
            //TODO: set buttons and other elements but chat to readonly 
        }

    }
}
