namespace AppExp.Models
{
    public class Card
    {
        public string Name { get; set; }
        public CardType Type { get; set; }
        public string ImagePath
        {
            get
            {
                return $"Resources/Images/{Type}.png";
            }
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
