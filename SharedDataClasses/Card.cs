
using System;
using System.IO;
using System.Net.Security;

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
        public Color color { get; set; }
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
            string name = color.ToString().ToLower() + number + ".png";
            string path = Path.GetFullPath(name);
            SourcePath = path;
        }

        public void setColor(Color color)
        {
            this.color = color;
            GetSourcepath();
        }

    }
}
