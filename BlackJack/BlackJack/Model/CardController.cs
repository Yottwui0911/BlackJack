﻿using System;
using System.Collections.Generic;
using System.Linq;
using BlackJack.Extentions;

namespace BlackJack.Model
{
    public abstract class CardController
    {
        /// <summary>
        /// 山札のカード
        /// </summary>
        protected List<Card> Cards { get; set; } = new List<Card>();

        /// <summary>
        /// 初期化
        /// </summary>
        public virtual void Initialize(bool hasJoker = false)
        {
            var cards = new List<Card>();

            foreach (Suit fragment in Enum.GetValues(typeof(Suit)))
            {
                for (var i = 1; i <= Card.CardMaxNumber; i++)
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
            this.Cards = Shuffle(cards);
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
            this.Cards = Shuffle(this.Cards);
        }

        public void Draw(ICollection<Card> hands)
        {
            var card = this.Cards.FirstOrDefault();
            hands.Add(card);
            this.Cards.Remove(card);
        }
    }
}