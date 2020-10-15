using System;
using System.Collections.Generic;
using System.Text;

namespace UNO
{
    public class User
    {
        private string Username { get; set; }
        private Boolean isHost;
        private List<Card> currentHand { get; set; }
    }
}
