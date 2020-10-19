
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

        public Card(Color color, int number)
        {
            this.number = number;
            this.color = color;
        }
    }
}
