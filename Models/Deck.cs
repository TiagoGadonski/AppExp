namespace AppExp.Models
{
    public class Deck
    {
        private Stack<Card> _cards;
        private List<Card> _discardPile;

        public Deck()
        {
            _cards = new Stack<Card>();
            _discardPile = new List<Card>();
        }

        public void InitializeDeck(int numberOfPlayers)
        {
            _cards.Clear();
            _discardPile.Clear();

            // Adicionar cartas ao baralho
            int totalDefuses = 6 - numberOfPlayers; // restam defuses após dar 1 a cada jogador
            int totalOtherCards = (numberOfPlayers * 7); // 7 cartas para cada jogador

            for (int i = 0; i < totalDefuses; i++) _cards.Push(new Card { Name = "Defuse", Type = CardType.Defuse });
            for (int i = 0; i < 5; i++) _cards.Push(new Card { Name = "Attack", Type = CardType.Attack });
            for (int i = 0; i < 5; i++) _cards.Push(new Card { Name = "Skip", Type = CardType.Skip });
            for (int i = 0; i < 4; i++) _cards.Push(new Card { Name = "Shuffle", Type = CardType.Shuffle });
            for (int i = 0; i < 5; i++) _cards.Push(new Card { Name = "See The Future", Type = CardType.SeeTheFuture });
            for (int i = 0; i < 5; i++) _cards.Push(new Card { Name = "Favor", Type = CardType.Favor });
            for (int i = 0; i < 5; i++) _cards.Push(new Card { Name = "Nope", Type = CardType.Nope });
            for (int i = 0; i < 5; i++) _cards.Push(new Card { Name = "TacoCat", Type = CardType.TacoCat });
            for (int i = 0; i < 5; i++) _cards.Push(new Card { Name = "HairyPotatoCat", Type = CardType.HairyPotatoCat });
            for (int i = 0; i < 5; i++) _cards.Push(new Card { Name = "RainbowRalphingCat", Type = CardType.RainbowRalphingCat });
            for (int i = 0; i < 5; i++) _cards.Push(new Card { Name = "BeardCat", Type = CardType.BeardCat });
            for (int i = 0; i < 5; i++) _cards.Push(new Card { Name = "Cattermelon", Type = CardType.Cattermelon });

            Shuffle();
        }

        public void AddExplodingKittens(int numberOfExplodingKittens)
        {
            for (int i = 0; i < numberOfExplodingKittens; i++)
            {
                _cards.Push(new Card { Name = "Exploding Kitten", Type = CardType.ExplodingKitten });
            }
            Shuffle();
        }

        public void Shuffle()
        {
            var cardsArray = _cards.ToArray();
            var random = new Random();
            _cards.Clear();
            foreach (var card in cardsArray.OrderBy(c => random.Next()))
            {
                _cards.Push(card);
            }
        }

        public Card DrawCard()
        {
            return _cards.Pop();
        }

        public void AddCard(Card card)
        {
            _cards.Push(card);
        }

        public void InsertCardAt(Card card, int position)
        {
            var cardsList = _cards.ToList();
            cardsList.Insert(position, card);
            _cards = new Stack<Card>(cardsList.Reverse<Card>());
        }

        public IEnumerable<Card> PeekTopCards(int count)
        {
            return _cards.Take(count);
        }

        public void DiscardCard(Card card)
        {
            _discardPile.Add(card);
        }

        public int CardsCount => _cards.Count;
    }
}
