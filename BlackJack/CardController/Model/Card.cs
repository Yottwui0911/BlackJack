using System.Collections.Generic;

namespace CardController.Model
{
    public class Card
    {
        /// <summary>
        /// 各Suitでの最高の値
        /// </summary>
        public static readonly int CardMaxNumber = 13;

        /// <summary>
        /// 俗称
        /// </summary>
        public string Call => $"{this.Suit}の{this.Number}";

        /// <summary>
        /// 引数なしのコンストラクタの場合、Joker扱いする
        /// </summary>
        public Card()
        {
            this.IsJoker = true;
        }

        /// <summary>
        /// コンストラクタ
        /// 数字とスートを決定する
        /// </summary>
        /// <param name="number"></param>
        /// <param name="suit"></param>
        public Card(int number, Suit suit)
        {
            this.Number = number;
            this.Suit = suit;
        }

        /// <summary>
        /// Jokerかどうか
        /// </summary>
        public bool IsJoker { get; }

        /// <summary>
        /// 数字
        /// </summary>
        public int Number { get; }

        private Suit m_suit;

        /// <summary>
        /// スート
        /// </summary>
        public Suit Suit
        {
            get => this.m_suit;
            set
            {
                this.m_suit = value;
                if (SuitColorMap.ContainsKey(value))
                {
                    this.Color = SuitColorMap[value];
                }
            }
        }

        /// <summary>
        /// スートの色
        /// </summary>
        public Color Color { get; private set; }

        /// <summary>
        /// エースかどうか
        /// </summary>
        public bool IsAce => this.Number == 1;

        /// <summary>
        /// ジャックかどうか
        /// </summary>
        public bool IsJack => this.Number == 11;

        /// <summary>
        /// クイーンかどうか
        /// </summary>
        public bool IsQueen => this.Number == 12;

        /// <summary>
        /// キングかどうか
        /// </summary>
        public bool IsKing => this.Number == 13;

        /// <summary>
        /// 絵札かどうか
        /// </summary>
        public bool IsPictureCards => this.IsJack || this.IsQueen || this.IsKing;

        private static readonly IDictionary<Suit, Color> SuitColorMap = new Dictionary<Suit, Color>
        {
            {Suit.Spade, Color.Black},
            {Suit.Heart, Color.Red},
            {Suit.Diamond, Color.Red},
            {Suit.Club, Color.Black},
        };
    }

    public enum Suit
    {
        Spade,
        Heart,
        Diamond,
        Club,
    }

    public enum Color
    {
        Black,
        Red,
    }
}
