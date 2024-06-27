using System.Collections.ObjectModel;

namespace AppExp.Models
{
    public class Player
    {
        public string Name { get; set; }
        public bool IsBot { get; set; }
        public int BotLevel { get; set; } // 0 = Humano, 1 = Fácil, 2 = Médio, 3 = Difícil
        public ObservableCollection<Card> Hand { get; set; }
        public bool IsMyTurn { get; set; }
        public int MustDrawCards { get; set; }  // Quantidade de cartas que o jogador deve pegar

        public Player()
        {
            Hand = new ObservableCollection<Card>();
            MustDrawCards = 1;  // Por padrão, cada jogador pega uma carta por turno
        }
    }
}
