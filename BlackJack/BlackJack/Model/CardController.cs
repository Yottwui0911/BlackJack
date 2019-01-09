using System;
using System.Collections.Generic;
using System.Linq;
using BlackJack.Extentions;

namespace BlackJack.Model
{
    public class CardController
    {
        public CardController()
        {
            this.Initialize();
        }

        /// <summary>
        /// 山札のカード
        /// </summary>
        private List<Card> m_cards = new List<Card>();

        /// <summary>
        /// ディーラーの手札
        /// </summary>
        private List<Card> DealerHands { get; } = new List<Card>();

        /// <summary>
        /// プレイヤーの手札
        /// </summary>
        private List<Card> PlayerHands { get; } = new List<Card>();

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize(bool hasJoker = false)
        {
            var cards = new List<Card>();

            foreach (Suit fragment in Enum.GetValues(typeof(Suit)))
            {
                for (var i = 1; i <= 13; i++)
                {
                    // それぞれのスートのカードを1～13まで用意する
                    cards.Add(new Card(i, fragment));
                }
            }

            if (hasJoker)
            {
                // Jokerを山札に混ぜる
                cards.Add(new Card());
            }

            // 山札をシャッフル
            this.m_cards = Shuffle(cards);

            // ディーラーが2枚ドローする
            this.DealerDraw(2);

            // プレイヤーが2枚ドローする
            this.PlayerDraw(2);
        }

        private static List<Card> Shuffle(IEnumerable<Card> cards)
        {
            return cards?.RandItems().ToList();
        }

        /// <summary>
        /// 山札をシャッフルする
        /// </summary>
        public void Shuffle()
        {
            this.m_cards = Shuffle(this.m_cards);
        }

        private void Draw(ICollection<Card> hands)
        {
            var card = this.m_cards.FirstOrDefault();
            hands.Add(card);
            this.m_cards.Remove(card);
        }

        /// <summary>
        /// プレイヤーがドローする
        /// </summary>
        /// <param name="count">ドローする枚数</param>
        public void PlayerDraw(int count)
        {
            for (var i = 0; i < count; i++)
            {
                this.Draw(this.PlayerHands);
            }
        }

        /// <summary>
        /// ディーラーがドローする
        /// </summary>
        /// <param name="count">ドローする枚数</param>
        public void DealerDraw(int count)
        {
            for (var i = 0; i < count; i++)
            {
                this.Draw(this.DealerHands);
            }
        }
    }
}
