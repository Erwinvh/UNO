
using System;

namespace SharedDataClasses
{

    public class Card
    {   
       public enum Color
    {
    BLACK, 
    RED,
    BLUE, 
    GREEN,
    YELLOW

    }
        public Color color
        {
            get { return color; }
            set
            {
                color = value;
                GetSourcepath();
            }
    }
        public int number { get; set; }
        public string SourcePath { get; set; }

        public Card(Color color, int number)
        {
            this.number = number;
            this.color = color;
            GetSourcepath();
        }

        public void GetSourcepath()
        {
            string name = @$"Cards\" + color.ToString().ToLower() + number + ".png";
            SourcePath = name;

        }

    }
}
