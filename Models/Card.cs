using System.ComponentModel;

namespace AppExp.Models
{
    public class Card : INotifyPropertyChanged
    {
        private bool _isSelected; 

        public string Name { get; set; }
        public CardType Type { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value) 
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public string ImagePath => $"Resources/Images/{Type}.png";

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum CardType
    {
        ExplodingKitten,
        Defuse,
        Attack,
        Skip,
        Shuffle,
        SeeTheFuture,
        TacoCat,
        HairyPotatoCat,
        RainbowRalphingCat,
        BeardCat,
        Cattermelon,
        Favor,
        Nope
    }
}
