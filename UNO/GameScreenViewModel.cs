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
using System.Windows;

namespace UNO
{
    public class GameScreenViewModel : INotifyPropertyChanged
    {
        public AsyncObservableCollection<Card> hand { get; set; }
        public AsyncObservableCollection<ChatMessage> ChatCollection { get; set; }
        public AsyncObservableCollection<User> userList { get; set; }
        public ICommand ChatCommand { get; set; }
        public ICommand DeckCommand { get; set; }
        public ICommand MoveCommand { get; set; }
        public ICommand quitGameCommand { get; set; }
        readonly App app;
        private NetworkCommunication networkCommunication;
        public string imageSource { get; set; }
        public string Message { get; set; } = "";
        public string PlayerPlayingName { get; set; }
        public string colorPicker { get; set; } = null;
        

        public event PropertyChangedEventHandler PropertyChanged;
        public bool isPlaying { get; set; }
        public Card pileCard { get; set; }
        public bool gameover = false;

        public GameScreenViewModel(App app, NetworkCommunication networkCommunication)
        {
            this.networkCommunication = networkCommunication;
            this.networkCommunication.GameScreenViewModel = this;
            this.app = app;
            isPlaying = false;
            hand = new AsyncObservableCollection<Card>();
            ChatCollection = new AsyncObservableCollection<ChatMessage>();
            userList = new AsyncObservableCollection<User>();
            ChatCommand = new RelayCommand(() => { sendChatmessage(Message); });
            DeckCommand = new RelayCommand(() => { pullFromDeck(); });
            MoveCommand = new RelayCommand<string>(sendMove);
            quitGameCommand = new RelayCommand(quitGame);
            imageSource = $@"/Cards/uno.png";
        }

        // 
        //--Game related-- 
        // 
        public void gameOver()
        {
            gameover = true;
        }

        public void RemovePlayer(string username)
        {
            foreach (User u in userList)
            {
                if (u.name.Equals(username))
                {
                    userList.Remove(u);
                    return;
                }
            }
        }

        public void pullFromDeck()
        {
            if (isPlaying)
            {
                networkCommunication.sendEmptyMove();
            }
        }
        public void addCardToUI(Card card)
        {
            hand.Add(card);
        }

        public void removeCardFromUI(Card removeCard)
        {
            int removingindex = -1;
            foreach (Card card in hand)
            {
                if (card.number == removeCard.number && removeCard.color == card.color)
                {
                    removingindex = hand.IndexOf(card);
                }
                else if (card.number == removeCard.number && card.color == Card.Color.BLACK)
                {
                    removingindex = hand.IndexOf(card);
                }
            }

            if (removingindex != -1)
            {
                hand.RemoveAt(removingindex);
            }
        }

        public void changePileCard(Card card)
        {
            imageSource = card.SourcePath;
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
                if ((movedCard.number == 13 || movedCard.number == 14) && movedCard != null)
                {

                    ChooseColor chooseColor = new ChooseColor(this);
                    chooseColor.ShowDialog();


                    //while (chooseColor.color == null)
                    //{

                    //}

                    if (colorPicker == null)
                    {
                        return;
                    }
                    
                    switch (colorPicker)
                    {
                        case "red":
                            movedCard.setColor(Card.Color.RED);
                            MessageBox.Show(movedCard.SourcePath);
                            break;
                        case "blue":
                            movedCard.setColor(Card.Color.BLUE);
                            break;
                        case "green":
                            movedCard.setColor(Card.Color.GREEN);
                            break;
                        case "yellow":
                            movedCard.setColor(Card.Color.YELLOW);
                            break;
                    }
                    colorPicker = null;
                }
                networkCommunication.sendMove(movedCard); 

            }
        }

        internal void AddMultpileCards(List<Card> added)
        {
            if (added!=null)
            {
                foreach (Card card in added)
                {
                    addCardToUI(card);
                }
            }
            
        }


        public void clearData()
        {
            hand.Clear();
            ChatCollection.Clear();
        }

        internal void EmptyHand()
        {
            
        }


        //
        //--Chat related--
        //

 public void receiverChatMessage(ChatMessage message)
        {
            if (message.sender == networkCommunication.user.name)
            {
                message.sender = null;
            }
            ChatCollection.Add(message);
        }

        public void sendChatmessage(string Message)
        {
            networkCommunication.sendChat(Message);
            this.Message = "";
        }

        public void quitGame()
        {
            clearData();
            if (gameover)
            {
                
                networkCommunication.resetToLobby();
                if (!app.Dispatcher.CheckAccess())
                {
                    app.Dispatcher.InvokeAsync(new Action(app.ReturnToLobby));
                }
                return;
            }
            networkCommunication.resetToLogin();
            app.ReturnToLogin();
            
        }

        //
        //--Other UI elements related--
        //
        public void transportPlayers(AsyncObservableCollection<User> players)
        {
            userList = players;
        }

        public void setPlayingState(bool isPlaying)
        {
            this.isPlaying = isPlaying;
        }
        
        public void changePlayerPlayingName(string name)
        {
            if (name == networkCommunication.user.name)
            {
                name = "You";
            }
            PlayerPlayingName = name;
        }

        public void editPlayerCardsInfo(string name, int amount)
        {
            foreach (User player in userList)
            {
                if (player.name == name)
                {
                    player.amountOfCards += amount;
                }
            }
        }

        public void MakeMessageBox(string message)
        {
            MessageBox.Show(message);
        }


        public void ResetCardAmounts()
        {
            foreach (User user in userList)
            {
                if (user.amountOfCards!=7)
                {
                    user.amountOfCards = 7;
                }
            }

            gameover = false;
        }
    }
}
