using AppExp.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

        public GameViewModel()
        {
            // Construtor padrão necessário para XAML
        }

        public GameViewModel(int numPlayers, int numBots)
        {
            Players = new ObservableCollection<Player>();
            GameLog = new ObservableCollection<string>();
            StartGameCommand = new RelayCommand(_ => StartGame());
            DrawCardCommand = new RelayCommand(_ => DrawCard(), _ => CanDrawCard());
            RestartGameCommand = new RelayCommand(_ => RestartGame());
            _deck = new Deck();
            _deck.InitializeDeck(numPlayers + numBots);

            // Inicializa jogadores e bots
            for (int i = 1; i <= numPlayers; i++)
            {
                Players.Add(new Player { Name = $"Jogador {i}", IsBot = false });
            }

            for (int i = 1; i <= numBots; i++)
            {
                Players.Add(new Player { Name = $"Bot {i}", IsBot = true, BotLevel = i % 3 + 1 }); // Distribui os níveis dos bots
            }

            StatusMessage = "Jogo iniciado!";
            CurrentPlayer = Players[0];
            AddToLog("Jogo iniciado!");
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged(nameof(StatusMessage));
            }
        }

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

        public ICommand StartGameCommand { get; }
        public ICommand DrawCardCommand { get; }
        public ICommand RestartGameCommand { get; }

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

            // Adicionar cartas Exploding Kitten ao baralho após distribuir as cartas
            _deck.AddExplodingKittens(4);

            StatusMessage = "Jogo reiniciado! Cada jogador recebeu 1 Defuse e 7 cartas.";
            AddToLog(StatusMessage);
            CurrentPlayer = Players[0];
        }

        private bool CanDrawCard()
        {
            return !_gameOver && CurrentPlayer != null;
        }

        private void DrawCard()
        {
            if (CurrentPlayer != null && !_gameOver)
            {
                var drawnCard = _deck.DrawCard();
                CurrentPlayer.Hand.Add(drawnCard);
                AddToLog($"{CurrentPlayer.Name} pegou uma carta {drawnCard.Name}.");

                if (drawnCard.Type == CardType.ExplodingKitten)
                {
                    // Lógica para lidar com a carta Exploding Kitten
                    if (CurrentPlayer.Hand.Any(c => c.Type == CardType.Defuse))
                    {
                        // Jogador tem um Defuse, então remove o Defuse e coloca o Exploding Kitten de volta no baralho
                        var defuseCard = CurrentPlayer.Hand.First(c => c.Type == CardType.Defuse);
                        CurrentPlayer.Hand.Remove(defuseCard);
                        _deck.AddCard(drawnCard);
                        _deck.Shuffle();
                        StatusMessage = $"{CurrentPlayer.Name} usou uma carta Defuse!";
                        AddToLog(StatusMessage);
                    }
                    else
                    {
                        // Jogador perde
                        StatusMessage = $"{CurrentPlayer.Name} pegou um Exploding Kitten e perdeu!";
                        AddToLog(StatusMessage);
                        Players.Remove(CurrentPlayer);
                        if (Players.Count == 1)
                        {
                            StatusMessage = $"{Players[0].Name} ganhou o jogo!";
                            AddToLog(StatusMessage);
                            _gameOver = true;
                        }
                    }
                }
                else
                {
                    StatusMessage = $"{CurrentPlayer.Name} pegou uma carta {drawnCard.Name}.";
                }

                // Passa para o próximo jogador
                var currentIndex = Players.IndexOf(CurrentPlayer);
                var nextIndex = (currentIndex + 1) % Players.Count;
                CurrentPlayer = Players[nextIndex];
            }
        }

        private void RestartGame()
        {
            // Lógica para reiniciar o jogo
            Players.Clear();
            GameLog.Clear();
            _deck.InitializeDeck(Players.Count);
            StatusMessage = "Jogo reiniciado!";
            AddToLog(StatusMessage);
            // Reiniciar jogadores e bots (exemplo: 2 jogadores e 0 bots)
            InitializeGame(2, 0);
        }

        private void InitializeGame(int numPlayers, int numBots)
        {
            // Inicializa o baralho
            _deck.InitializeDeck(numPlayers + numBots);

            // Inicializa jogadores
            for (int i = 1; i <= numPlayers; i++)
            {
                Players.Add(new Player { Name = $"Jogador {i}", IsBot = false });
            }

            // Inicializa bots
            for (int i = 1; i <= numBots; i++)
            {
                Players.Add(new Player { Name = $"Bot {i}", IsBot = true, BotLevel = i % 3 + 1 });
            }

            StatusMessage = "Jogo reiniciado!";
            AddToLog(StatusMessage);
            CurrentPlayer = Players[0];

            // Distribuir cartas aos jogadores
            foreach (var player in Players)
            {
                player.Hand.Clear();
                player.Hand.Add(new Card { Name = "Defuse", Type = CardType.Defuse });
                for (int i = 0; i < 7; i++)
                {
                    player.Hand.Add(_deck.DrawCard());
                }
            }

            // Adicionar cartas Exploding Kitten ao baralho após distribuir as cartas
            _deck.AddExplodingKittens(4);
        }

        private void AddToLog(string message)
        {
            GameLog.Add(message);
            OnPropertyChanged(nameof(GameLog));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
