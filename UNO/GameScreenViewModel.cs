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
        public AsyncObservableCollection<Card> hand { get; set; }
        public ICommand ChatCommand { get; set; }
        public ICommand DeckCommand { get; set; }
        public ICommand MoveCommand { get; set; }
        readonly App app;
        private NetworkCommunication networkCommunication;
        public string imageSource { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public string Message { set; get; }
        public bool isPlaying { get; set; }

        public GameScreenViewModel(App app, NetworkCommunication networkCommunication)
        {
            this.networkCommunication = networkCommunication;
            this.networkCommunication.GameScreenViewModel = this;
            //this.LeaveLobbyCommand = new RelayCommand(() => { LeaveLobby(); }); 
            //this.LaunchGameCommand = new RelayCommand(() => { LaunchGame(); }); 
            this.app = app;
            hand = new AsyncObservableCollection<Card>();
            ChatCommand = new RelayCommand(() => { sendChatmessage(Message); });
            DeckCommand = new RelayCommand(() => { pullFromDeck(); });
            MoveCommand = new RelayCommand<string>(sendMove);
        }

        // 
        //--UI to Netrwerkcom-- 
        // 
        public void pullFromDeck()
        {
            if (isPlaying)
            {
                networkCommunication.sendEmptyMove();
            }
        }

        public void sendChatmessage(string message)
        {
            networkCommunication.sendChat(message);
        }

        public void sendMove(string playedCard)
        {
            if (isPlaying)
            {
                Card movedCard = null;
                foreach (Card card in hand)
                {
                    if (card.SourcePath == playedCard)
                    {
                        movedCard = card;
                    }
                }
                //TODO: add wildcard logic
            //if (playedCard.number == 13 || playedCard.number == 14)
            //{
            //    playedCard.color = color;
            //}
            networkCommunication.sendMove(movedCard); 

            }
        }


        // 
        //--netwerkcom to UI-- 
        // 
        public void addCardToUI(Card card)
        {
            //TODO: add card to UI 
            Debug.WriteLine("Card added" + card.number);
            hand.Add(card);
        }

        public void removeCardFromUI(Card card)
        {
            //TODO: try via index or via card 
            hand.Remove(card);
        }

        public void changePileCard(Card card)
        {
            //TODO: change pileCard 
        }

        public void setPlayingState(bool isPlaying)
        {
            this.isPlaying = isPlaying;
        }

        internal void AddMultpileCards(List<Card> added)
        {
            foreach (Card card in added)
            {
                addCardToUI(card);
            }
        }

        internal void EmptyHand()
        {
            hand.Clear();
        }
    }
}
