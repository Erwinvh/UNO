using System;
using System.Collections.Generic;
using System.Text;

namespace SharedDataClasses
{
    class Score
    {
        public string username { get; set; }
        public int winAmount { get; set; }
        public int gameAmount { get; set; }

        public Score(string name, int wins, int total)
        {
            username = name;
            winAmount = wins;
            gameAmount = total;
        }


    }
}
