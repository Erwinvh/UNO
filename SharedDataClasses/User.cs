
namespace SharedDataClasses
{
    public class User
    {
        
        public string name { get; set; }
        public bool isReady { get; set; }
        public int amountOfCards { get; set; }

        public User(string name)
        {
            this.name = name;
            isReady = false;
            amountOfCards = 7;
        }

    }
}
