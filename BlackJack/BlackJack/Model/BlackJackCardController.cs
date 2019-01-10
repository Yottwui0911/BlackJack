using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlackJack.Model
{
    public sealed class BlackJackCardController : CardController
    {
        #region consts

        public static class InputCommands
        {
            public static readonly string PlayerHand = "hand";
            public static readonly string DealerHand = "dhand";
            public static readonly string Help = "help";
            public static readonly string Draw = "draw";
            public static readonly string End = "end";
        }

        /// <summary>
        /// ディーラーが引く数字の閾値
        /// </summary>
        private static readonly int m_thresholdNum = 17;

        /// <summary>
        /// バーストの数値
        /// </summary>
        public static readonly int BurstNum = 21;

        #endregion

        #region properties

        private Actor m_winner;

        /// <summary>
        /// 勝者
        /// </summary>
        public Actor Winner
        {
            get => this.m_winner;
            private set
            {
                this.m_winner = value;
                this.IsGameEnd = true;
            }
        }

        /// <summary>
        /// ゲーム終了かどうか
        /// </summary>
        public bool IsGameEnd { get; set; }

        #region Messages

        private string DealerHandStr(bool isEnd = false)
        {
            var num = isEnd ? this.GetDealerHandsSum() : this.DealerHands.FirstOrDefault()?.Number;
            return $"Dealer:{num}";
        }

        private string PlayerHandStr => $"Player:{this.GetPlayerHandsSum()}";

        /// <summary>
        /// バーストしたときのメッセージ
        /// </summary>
        /// <param name="actor"></param>
        /// <returns></returns>
        private string BurstMsg(Actor actor) => $"{actor}がBurstしました\n{this.PlayerHandStr}\n{this.DealerHandStr(true)}\n";

        /// <summary>
        /// 許可されていないコマンドのメッセージ
        /// </summary>
        private static readonly string UnauthorizedMsg = "許されていないコマンドが入力されました。\nhelpを入力して、使い方を確認してください。\n";

        /// <summary>
        /// カードを引いた時のメッセージ
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="card"></param>
        /// <returns></returns>
        private static string DrawMsg(Actor actor, Card card) => $"{actor}が{card?.Suit}の{card?.Number}を引きました。\n";

        #endregion

        /// <summary>
        /// ディーラーの手札
        /// </summary>
        private List<Card> DealerHands { get; } = new List<Card>();

        /// <summary>
        /// プレイヤーの手札
        /// </summary>
        private List<Card> PlayerHands { get; } = new List<Card>();

        #endregion

        public BlackJackCardController()
        {
            this.Initialize();

            this.m_controller = new Dictionary<string, Func<string>>
            {
                {InputCommands.PlayerHand,this.DoPlayerHand },
                {InputCommands.DealerHand, this.DoDealerHand },
                {InputCommands.Help, DoHelp },
                {InputCommands.Draw, this.DoDraw },
                {InputCommands.End, this.DoEndGame },
            };
        }

        #region methods

        /// <inheritdoc />
        /// <summary>
        /// 初期化
        /// </summary>
        public override void Initialize(bool hasJoker = false)
        {
            base.Initialize(hasJoker);

            // ディーラーが2枚ドローする
            this.DealerDraw(2);

            // プレイヤーが2枚ドローする
            this.PlayerDraw(2);
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

        /// <summary>
        /// 手札を見て、値を取得
        /// </summary>
        /// <param name="hands"></param>
        /// <returns></returns>
        private static int GetCardsNumberSum(IEnumerable<Card> hands)
        {
            var enumerable = hands.ToList();
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
                if (val <= BurstNum)
                {
                    break;
                }

                val = val - 10;
            }
            return val;
        }
        
        /// <summary>
        /// プレイヤーの合計の数値
        /// </summary>
        /// <returns></returns>
        public int GetPlayerHandsSum()
        {
            return GetCardsNumberSum(this.PlayerHands);
        }

        /// <summary>
        /// ディーラーの合計の数値
        /// </summary>
        /// <returns></returns>
        public int GetDealerHandsSum()
        {
            return GetCardsNumberSum(this.DealerHands);
        }

        /// <summary>
        /// 入力
        /// </summary>
        /// <param name="input"></param>
        public string Input(string input)
        {
            return !this.m_controller.ContainsKey(input)
                ? UnauthorizedMsg
                : this.m_controller[input]();
        }

        private readonly IDictionary<string, Func<string>> m_controller;

        private string DoPlayerHand()
        {
            return string.Join("\n", this.PlayerHands.Select(x => $"{x.Call}")) + $"\n{this.PlayerHandStr}\n";
        }

        private string DoDealerHand()
        {
            return this.DealerHands.FirstOrDefault()?.Call + $"\n{this.DealerHandStr()}\n";
        }

        private static string DoHelp()
        {
            return "■使い方\n" +
                   "以下のキーを入力してください。\n" +
                   $"・{InputCommands.PlayerHand}:自分の手札を確認できます\n" +
                   $"・{InputCommands.DealerHand}:ディーラーの手札を確認できます\n" +
                   $"・{InputCommands.Draw}:手札を山札から引きます\n" +
                   $"・{InputCommands.End}:終了して、ディーラーと勝負します\n";
        }

        private string DoDraw()
        {
            // プレイヤーが一枚ドロー
            this.PlayerDraw(1);
            var msg = DrawMsg(Actor.Player, this.PlayerHands.LastOrDefault());

            if (this.GetPlayerHandsSum() <= BurstNum)
            {
                // ドローしたカード表示
                return msg + $"{this.PlayerHandStr}\n";
            }

            // バーストしたらディーラーの勝利
            this.Winner = Actor.Dealer;
            return msg + this.BurstMsg(Actor.Player);

        }

        /// <summary>
        /// 最後にディーラーがドローする
        /// </summary>
        private string DoEndGame()
        {
            var msg = new StringBuilder();
            while (this.GetDealerHandsSum() < m_thresholdNum)
            {
                this.DealerDraw(1);
                msg.Append(DrawMsg(Actor.Dealer, this.DealerHands.LastOrDefault()));
            }

            if (this.GetDealerHandsSum() > BurstNum)
            {
                // バーストしたらプレイヤーの勝利
                this.Winner = Actor.Player;
                msg.Append(this.BurstMsg(Actor.Dealer));
                return msg.ToString();
            }

            // どちらもバーストしていないので、手札の数値で勝負
            this.Winner = this.GetPlayerHandsSum() > this.GetDealerHandsSum()
                ? Actor.Player
                : Actor.Dealer;
            msg.Append($"{this.PlayerHandStr}\n{this.DealerHandStr(true)}\n");
            return msg.ToString();
        }

        #endregion
    }

    public enum Actor
    {
        Player,
        Dealer,
    }
}
