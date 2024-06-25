using System.Collections.ObjectModel;

namespace AppExp.Models
{
    public class Player
    {
        public string Name { get; set; }
        public bool IsBot { get; set; }
        public int BotLevel { get; set; } // 0 = Humano, 1 = Fácil, 2 = Médio, 3 = Difícil
        public ObservableCollection<Card> Hand { get; set; }

        public Player()
        {
            Hand = new ObservableCollection<Card>();
        }

    }
}
