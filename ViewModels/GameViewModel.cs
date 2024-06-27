using AppExp.Models;
using AppExp.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace AppExp.ViewModels
{
    public class GameViewModel : INotifyPropertyChanged
    {
        private string _statusMessage;
        private ObservableCollection<Player> _players;
        private Player _currentPlayer;
        private Deck _deck;
        private bool _gameOver;
        private bool _mustDrawExtraCard;

        public GameViewModel() { }

        public GameViewModel(int numPlayers, int numBots)
        {
            Players = new ObservableCollection<Player>();
            GameLog = new ObservableCollection<string>();
            StartGameCommand = new RelayCommand(_ => StartGame());
            DrawCardCommand = new RelayCommand(_ => DrawCard(), _ => CanDrawCard());
            RestartGameCommand = new RelayCommand(_ => RestartGame());
            PlayCardCommand = new RelayCommand(CardAction);
            _deck = new Deck();

            InitializeGame(numPlayers, numBots);
            StatusMessage = "Preparando o jogo...";
            AddToLog("Inicializando o jogo...");
        }

        public ICommand StartGameCommand { get; }
        public ICommand DrawCardCommand { get; }
        public ICommand RestartGameCommand { get; }
        public ICommand PlayCardCommand { get; }

        public ObservableCollection<Player> Players
        {
            get => _players;
            set
            {
                _players = value;
                OnPropertyChanged(nameof(Players));
            }
        }

        public Player CurrentPlayer
        {
            get => _currentPlayer;
            set
            {
                _currentPlayer = value;
                OnPropertyChanged(nameof(CurrentPlayer));
            }
        }

        public ObservableCollection<string> GameLog { get; set; }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged(nameof(StatusMessage));
            }
        }

        private void AddToLog(string message)
        {
            GameLog.Add(message);
            OnPropertyChanged(nameof(GameLog));
        }

        private void CardAction(object parameter)
        {
            if (parameter is Card playedCard)
            {
                if (CurrentPlayer.Hand.Contains(playedCard))
                {
                    if (IsCatCard(playedCard.Type) && HasMatchingCatCard(playedCard.Type))
                    {
                        PlayCatCard(playedCard);
                    }
                    else
                    {
                        CurrentPlayer.Hand.Remove(playedCard);
                        _deck.DiscardCard(playedCard);
                        AddToLog($"{CurrentPlayer.Name} jogou a carta {playedCard.Name}.");
                        ApplyCardEffect(playedCard);
                    }
                }
            }
        }

        private bool IsCatCard(CardType type)
        {
            return type == CardType.TacoCat ||
                   type == CardType.HairyPotatoCat ||
                   type == CardType.RainbowRalphingCat ||
                   type == CardType.BeardCat ||
                   type == CardType.Cattermelon;
        }

        private bool HasMatchingCatCard(CardType type)
        {
            return CurrentPlayer.Hand.Count(c => c.Type == type) > 1;
        }

        private void PlayCatCard(Card playedCard)
        {
            var matchingCard = CurrentPlayer.Hand.First(c => c.Type == playedCard.Type && c != playedCard);
            CurrentPlayer.Hand.Remove(playedCard);
            CurrentPlayer.Hand.Remove(matchingCard);
            _deck.DiscardCard(playedCard);
            _deck.DiscardCard(matchingCard);
            AddToLog($"{CurrentPlayer.Name} jogou duas cartas {playedCard.Name}.");

            // Prompt to select a player to steal a card from
            var otherPlayers = Players.Where(p => p != CurrentPlayer && p.Hand.Any()).ToList();
            if (otherPlayers.Any())
            {
                var selectPlayerDialog = new SelectPlayerDialog(new ObservableCollection<Player>(otherPlayers));
                if (selectPlayerDialog.ShowDialog() == true)
                {
                    var selectedPlayer = selectPlayerDialog.SelectedPlayer;
                    var stolenCard = StealRandomCard(selectedPlayer);
                    AddToLog($"{CurrentPlayer.Name} roubou uma carta {stolenCard.Name} de {selectedPlayer.Name}.");
                }
            }

            OnPropertyChanged(nameof(CurrentPlayer));
        }

        private Card StealRandomCard(Player player)
        {
            var random = new Random();
            int index = random.Next(player.Hand.Count);
            var card = player.Hand[index];
            player.Hand.RemoveAt(index);
            CurrentPlayer.Hand.Add(card);
            return card;
        }

        private void ApplyCardEffect(Card card)
        {
            switch (card.Type)
            {
                case CardType.Attack:
                    _mustDrawExtraCard = true;
                    AddToLog($"{CurrentPlayer.Name} usou a carta Ataque!");
                    NextPlayer();
                    break;
                case CardType.Skip:
                    AddToLog($"{CurrentPlayer.Name} usou a carta Pular!");
                    NextPlayer();
                    break;
                case CardType.SeeTheFuture:
                    ViewTopCards();
                    break;
                case CardType.Shuffle:
                    _deck.Shuffle();
                    AddToLog("O baralho foi embaralhado.");
                    break;
                case CardType.Favor:
                    PlayFavorCard();
                    break;
                case CardType.Nope:
                    // Implement logic for Nope card
                    AddToLog($"{CurrentPlayer.Name} usou a carta Nope!");
                    break;
                default:
                    // Other card types logic
                    break;
            }
            OnPropertyChanged(nameof(StatusMessage));
        }

        private void PlayFavorCard()
        {
            var otherPlayers = Players.Where(p => p != CurrentPlayer && p.Hand.Any()).ToList();
            if (otherPlayers.Any())
            {
                var selectPlayerDialog = new SelectPlayerDialog(new ObservableCollection<Player>(otherPlayers));
                if (selectPlayerDialog.ShowDialog() == true)
                {
                    var selectedPlayer = selectPlayerDialog.SelectedPlayer;
                    var stolenCard = StealRandomCard(selectedPlayer);
                    AddToLog($"{CurrentPlayer.Name} pediu um Favor e {selectedPlayer.Name} deu a carta {stolenCard.Name}.");
                }
            }
        }

        private void ViewTopCards()
        {
            var topCards = _deck.PeekTopCards(3);
            AddToLog($"{CurrentPlayer.Name} viu as próximas cartas: {string.Join(", ", topCards.Select(c => c.Name))}");
        }

        private void NextPlayer()
        {
            int currentIndex = Players.IndexOf(CurrentPlayer);
            int nextIndex = (currentIndex + 1) % Players.Count;
            CurrentPlayer = Players[nextIndex];
            if (_mustDrawExtraCard)
            {
                _mustDrawExtraCard = false;
                CurrentPlayer.MustDrawCards = 2;
            }
            else
            {
                CurrentPlayer.MustDrawCards = 1;
            }
            OnPropertyChanged(nameof(CurrentPlayer));
        }

        private void StartGame()
        {
            _gameOver = false;
            _deck.InitializeDeck(Players.Count);
            foreach (var player in Players)
            {
                player.Hand.Clear();
                player.Hand.Add(new Card { Name = "Defuse", Type = CardType.Defuse });
                for (int i = 0; i < 7; i++)
                {
                    player.Hand.Add(_deck.DrawCard());
                }
            }

            _deck.AddExplodingKittens(Players.Count - 1);
            _deck.Shuffle();

            if (Players.Any())
            {
                CurrentPlayer = Players[0];
            }

            StatusMessage = "Jogo iniciado!";
            AddToLog(StatusMessage);
            OnPropertyChanged(nameof(Players));
        }

        private bool CanDrawCard()
        {
            return !_gameOver && CurrentPlayer != null && CurrentPlayer.MustDrawCards > 0;
        }

        private void DrawCard()
        {
            if (CurrentPlayer != null && !_gameOver)
            {
                var drawnCard = _deck.DrawCard();
                CurrentPlayer.Hand.Add(drawnCard);
                AddToLog($"{CurrentPlayer.Name} pegou uma carta {drawnCard.Name}.");
                CurrentPlayer.MustDrawCards--;

                if (drawnCard.Type == CardType.ExplodingKitten)
                {
                    if (CurrentPlayer.Hand.Any(c => c.Type == CardType.Defuse))
                    {
                        var defuseCard = CurrentPlayer.Hand.First(c => c.Type == CardType.Defuse);
                        CurrentPlayer.Hand.Remove(defuseCard);
                        AddToLog($"{CurrentPlayer.Name} desarmou um Exploding Kitten!");
                        PromptExplodingKittenPlacement();
                    }
                    else
                    {
                        StatusMessage = $"{CurrentPlayer.Name} explodiu!";
                        AddToLog(StatusMessage);
                        Players.Remove(CurrentPlayer);
                        if (Players.Count == 1)
                        {
                            StatusMessage = $"{Players[0].Name} é o vencedor!";
                            AddToLog(StatusMessage);
                            _gameOver = true;
                        }
                        else
                        {
                            NextPlayer();
                        }
                    }
                }
                else
                {
                    if (CurrentPlayer.MustDrawCards == 0)
                    {
                        NextPlayer();
                    }
                }

                OnPropertyChanged(nameof(CurrentPlayer));
            }
        }

        private void PromptExplodingKittenPlacement()
        {
            var inputBox = new InputBox("Digite a posição para colocar a carta Exploding Kitten (1 a " + (_deck.CardsCount + 1) + "):");
            if (inputBox.ShowDialog() == true)
            {
                int position = int.Parse(inputBox.ResponseText) - 1;
                _deck.InsertCardAt(new Card { Name = "Exploding Kitten", Type = CardType.ExplodingKitten }, position);
                AddToLog($"{CurrentPlayer.Name} colocou a carta Exploding Kitten de volta no baralho na posição {position + 1}.");
                if (CurrentPlayer.MustDrawCards == 0)
                {
                    NextPlayer();
                }
            }
        }

        private void RestartGame()
        {
            var startUpWindow = new StartupWindow();
            Application.Current.Windows.OfType<MainWindow>().FirstOrDefault()?.Close();
            startUpWindow.Show();
            StatusMessage = "Jogo reiniciado!";
            AddToLog(StatusMessage);
        }

        private void InitializeGame(int numPlayers, int numBots)
        {
            Players.Clear();
            _deck.InitializeDeck(numPlayers + numBots);

            for (int i = 1; i <= numPlayers; i++)
            {
                Players.Add(new Player { Name = $"Jogador {i}", IsBot = false });
            }
            for (int i = 1; i <= numBots; i++)
            {
                Players.Add(new Player { Name = $"Bot {i}", IsBot = true, BotLevel = i % 3 + 1 });
            }

            foreach (var player in Players)
            {
                player.Hand.Add(new Card { Name = "Defuse", Type = CardType.Defuse });
                for (int i = 0; i < 7; i++)
                {
                    player.Hand.Add(_deck.DrawCard());
                }
            }

            _deck.AddExplodingKittens(Players.Count - 1);
            _deck.Shuffle();

            if (Players.Any())
            {
                CurrentPlayer = Players[0];
            }

            StatusMessage = "Jogo iniciado!";
            AddToLog(StatusMessage);
            OnPropertyChanged(nameof(CurrentPlayer));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
