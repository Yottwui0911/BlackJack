using System.Linq;

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
            var val = enumerable.Sum(x =>
            {
                // 絵柄は10の扱い
                if (x.IsPictureCards)
                {
                    return 10;
                }

                // エースは11として扱う
                if (x.IsAce)
                {
                    return 11;
                }

                // そのほかは普通の数字
                return x.Number;
            });

            var aceCount = enumerable.Count(x => x.IsAce);

            return ConsiderAceValue(val, aceCount);
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
        /// <param name="controller"></param>
        /// <param name="count"></param>
        public void Draw(CardController controller, int count)
        {
            for (var i = 0; i < count; i++)
            {
                controller.Draw(this.Hands);
            }
        }
    }
}
