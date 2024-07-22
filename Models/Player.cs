using System.Collections.ObjectModel;

namespace AppExp.Models
{
    public class Player
    {
        public string Name { get; set; }
        public bool IsBot { get; set; }
        public int BotLevel { get; set; } 
        public ObservableCollection<Card> Hand { get; set; }
        public bool IsMyTurn { get; set; }
        public int MustDrawCards { get; set; }  

        public Player()
        {
            Hand = new ObservableCollection<Card>();
            MustDrawCards = 1;
        }
    }
}
