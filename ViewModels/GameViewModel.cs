using AppExp.Models;
using AppExp.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
        private Card _selectedCard;

        public GameViewModel() { }

        public GameViewModel(int numPlayers, int numBots)
        {
            Players = new ObservableCollection<Player>();
            GameLog = new ObservableCollection<string>();
            DrawCardCommand = new RelayCommand(_ => DrawCard(), _ => CanDrawCard());
            RestartGameCommand = new RelayCommand(_ => RestartGame());
            PlaySelectedCardCommand = new RelayCommand(_ => PlaySelectedCard(), _ => CanPlaySelectedCard);
            SelectCardCommand = new RelayCommand(SelectCard);

            _deck = new Deck();

            InitializeGame(numPlayers, numBots);
            StatusMessage = "Preparando o jogo...";
            AddToLog("Inicializando o jogo...");
        }

        public ICommand DrawCardCommand { get; private set; }
        public ICommand RestartGameCommand { get; private set; }
        public ICommand PlaySelectedCardCommand { get; private set; }
        public ICommand SelectCardCommand { get; private set; }

        public ObservableCollection<Player> Players
        {
            get => _players;
            set
            {
                _players = value;
                OnPropertyChanged(nameof(Players));
            }
        }

        private void SelectCard(object card)
        {
            SelectedCard = card as Card;
            if (IsCatCard(SelectedCard.Type) && HasMatchingCatCard(SelectedCard.Type))
            {
                PlayCatCard(SelectedCard);
            }
        }

        public Player CurrentPlayer
        {
            get => _currentPlayer;
            set
            {
                _currentPlayer = value;
                OnPropertyChanged(nameof(CurrentPlayer));
                OnPropertyChanged(nameof(IsCurrentPlayer));
                NotifyCommands();
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

        public Card SelectedCard
        {
            get => _selectedCard;
            set
            {
                if (_selectedCard != value)
                {
                    _selectedCard = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanPlaySelectedCard));
                    ((RelayCommand)PlaySelectedCardCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public bool IsCurrentPlayer => CurrentPlayer != null && !CurrentPlayer.IsBot;

        private void NotifyCommands()
        {
            ((RelayCommand)PlaySelectedCardCommand).NotifyCanExecuteChanged();
            ((RelayCommand)DrawCardCommand).NotifyCanExecuteChanged();
            CommandManager.InvalidateRequerySuggested();
        }


        private void AddToLog(string message)
        {
            GameLog.Add(message);
            OnPropertyChanged(nameof(GameLog));
        }

        private void PlaySelectedCard()
        {
            if (SelectedCard != null && CurrentPlayer != null && CurrentPlayer.Hand.Contains(SelectedCard))
            {
                if (IsCatCard(SelectedCard.Type))
                {
                    PlayCatCard(SelectedCard);
                }
                else
                {

                    CurrentPlayer.Hand.Remove(SelectedCard);
                    _deck.DiscardCard(SelectedCard);
                    AddToLog($"{CurrentPlayer.Name} jogou a carta {SelectedCard.Name}.");
                    ApplyCardEffect(SelectedCard);
                }

                SelectedCard = null; 
                OnPropertyChanged(nameof(SelectedCard)); 
                NotifyCommands(); 
            }
        }

        public bool CanPlaySelectedCard
        {
            get { return CanPlaySelectedCardEvaluate(); }
        }

        private bool CanPlaySelectedCardEvaluate()
        {
            return SelectedCard != null && CurrentPlayer != null && CurrentPlayer.Hand.Contains(SelectedCard) && IsCurrentPlayer &&
           (!IsCatCard(SelectedCard.Type) || HasMatchingCatCard(SelectedCard.Type));
        }

        private void CardAction(object parameter)
        {
            if (parameter is Card playedCard && CurrentPlayer != null && CurrentPlayer.Hand.Contains(playedCard))
            {
                CurrentPlayer.Hand.Remove(playedCard);
                _deck.DiscardCard(playedCard);
                AddToLog($"{CurrentPlayer.Name} jogou a carta {playedCard.Name}.");
                ApplyCardEffect(playedCard);
                OnPropertyChanged(nameof(CurrentPlayer));
                NotifyCommands();
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
            var matchingCards = CurrentPlayer.Hand.Where(c => c.Type == playedCard.Type).ToList();
            var otherPlayers = Players.Where(p => p != CurrentPlayer && p.Hand.Any()).ToList();

            if (matchingCards.Count >= 2 && matchingCards.Count <= 3 && otherPlayers.Any())
            {
                var selectPlayerDialog = new SelectPlayerDialog(new ObservableCollection<Player>(otherPlayers));
                if (selectPlayerDialog.ShowDialog() == true)
                {
                    var selectedPlayer = selectPlayerDialog.SelectedPlayer;
                    if (matchingCards.Count == 2)
                    {
                        var stolenCard = StealRandomCard(selectedPlayer);
                        AddToLog($"{CurrentPlayer.Name} roubou uma carta aleatória {stolenCard.Name} de {selectedPlayer.Name}.");
                    }
                    else if (matchingCards.Count == 3)
                    {
                        // Logic to select a specific card goes here
                        // Placeholder: Steal specific card (to be implemented)
                        AddToLog($"{CurrentPlayer.Name} escolheu uma carta específica de {selectedPlayer.Name}.");
                    }
                }
            }
            else if (matchingCards.Count == 5 && HasAllDifferentCatCards())
            {
                AddToLog($"{CurrentPlayer.Name} escolheu uma carta do monte de descarte.");
            }
            else
            {
                MessageBox.Show("Combinação de cartas inválida. Verifique as regras do jogo.");
            }

            if (matchingCards.Count == 2 || (matchingCards.Count == 3 && otherPlayers.Any()) || (matchingCards.Count == 5 && HasAllDifferentCatCards()))
            {
                CurrentPlayer.Hand.Remove(playedCard);
                _deck.DiscardCard(playedCard);
                AddToLog($"{CurrentPlayer.Name} jogou a carta {playedCard.Name}.");
            }
        }

        private bool HasAllDifferentCatCards()
        {
            var catCardTypes = new List<CardType>
            {
                CardType.TacoCat,
                CardType.HairyPotatoCat,
                CardType.RainbowRalphingCat,
                CardType.BeardCat,
                CardType.Cattermelon
            };

            return catCardTypes.All(type => CurrentPlayer.Hand.Any(c => c.Type == type));
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
            ((RelayCommand)PlaySelectedCardCommand).NotifyCanExecuteChanged();
            NotifyCommands();
        }

        private bool CanDrawCard()
        {
            return !_gameOver && CurrentPlayer != null && CurrentPlayer.MustDrawCards > 0;
        }

        private void DrawCard()
        {
            if (CurrentPlayer != null && !_gameOver && CurrentPlayer.MustDrawCards > 0)
            {
                var drawnCard = _deck.DrawCard();
                CurrentPlayer.Hand.Add(drawnCard);
                AddToLog($"{CurrentPlayer.Name} pegou uma carta {drawnCard.Name}.");
                CurrentPlayer.MustDrawCards--;

                if (drawnCard.Type == CardType.ExplodingKitten)
                {
                    HandleExplodingKitten(drawnCard);
                }
                else
                {
                    CheckForTurnPass();
                }

                OnPropertyChanged(nameof(CurrentPlayer));
                NotifyCommands(); 
            }
        }



        private void ExplodePlayer()
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
            NextPlayer();
        }

        private void CheckForTurnPass()
        {
            if (CurrentPlayer.MustDrawCards <= 0)
            {
                NextPlayer();
            }
        }

        private void HandleExplodingKitten(Card drawnCard)
        {
            if (CurrentPlayer.Hand.Any(c => c.Type == CardType.Defuse))
            {
                var defuseCard = CurrentPlayer.Hand.First(c => c.Type == CardType.Defuse);
                CurrentPlayer.Hand.Remove(defuseCard);
                _deck.DiscardCard(defuseCard); 

                CurrentPlayer.Hand.Remove(drawnCard);

                PromptExplodingKittenPlacement(drawnCard); 
            }
            else
            {
                ExplodePlayer();
            }
        }

        private void PromptExplodingKittenPlacement(Card kittenCard)
        {
            var inputBox = new InputBox("Digite a posição para colocar a carta Exploding Kitten (1 a " + (_deck.CardsCount + 1) + "):");
            if (inputBox.ShowDialog() == true)
            {
                int position = int.Parse(inputBox.ResponseText) - 1;
                _deck.InsertCardAt(kittenCard, position);
                AddToLog($"{CurrentPlayer.Name} colocou a carta Exploding Kitten de volta no baralho na posição {position + 1}.");
                NextPlayer();
            }
            else
            {
                NextPlayer(); 
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
            NotifyCommands();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (propertyName == nameof(SelectedCard) || propertyName == nameof(CurrentPlayer))
            {
                OnPropertyChanged(nameof(CanPlaySelectedCard)); 
            }
        }
    }
}

