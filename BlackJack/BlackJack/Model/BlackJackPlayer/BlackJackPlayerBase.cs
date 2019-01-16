using System.Linq;
using CardController.Model;

namespace BlackJack.Model.BlackJackPlayer
{
    public class BlackJackPlayerBase : PlayerBase
    {
        public virtual string ShowHand()
        {
            return string.Empty;
        }

        /// <summary>
        /// 手札を見て、値を取得
        /// </summary>
        /// <returns></returns>
        public int GetCardsNumberSum()
        {
            var enumerable = this.Hands.ToList();
            var val = enumerable.Sum(x => this.GetBlackJackNum(x));

            var aceCount = enumerable.Count(x => x.IsAce);

            return ConsiderAceValue(val, aceCount);
        }

        /// <summary>
        /// BlackJack専用の数値の扱い
        /// </summary>
        /// <returns></returns>
        protected int GetBlackJackNum(Card card)
        {
            if(card == null)
            {
                return 0;
            }

            // 絵柄は10の扱い
            if (card.IsPictureCards)
            {
                return 10;
            }

            // エースは11として扱う
            if (card.IsAce)
            {
                return 11;
            }

            // そのほかは普通の数字
            return card.Number;
        }

        /// <summary>
        /// 21を超えている場合、エースは1とカウントできるようにする
        /// </summary>
        /// <param name="val"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private static int ConsiderAceValue(int val, int count)
        {
            for (var i = 0; i < count; i++)
            {
                if (val <= BlackJackCardController.BurstNum)
                {
                    break;
                }

                val = val - 10;
            }
            return val;
        }

        /// <summary>
        /// 山札からカードをドローする
        /// </summary>
        /// <param name="deck"></param>
        /// <param name="count"></param>
        public void Draw(Deck deck, int count)
        {
            for (var i = 0; i < count; i++)
            {
                var card = deck.Cards.FirstOrDefault();
                this.Hands.Add(card);
                deck.Cards.Remove(card);
            }
        }
    }
}
