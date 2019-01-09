using System.Collections.Generic;
using System.Linq;

namespace BlackJack.Model
{
    public sealed class BlackJackCardController : CardController
    {
        #region consts

        private static class InputCommands
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
        private static readonly int m_burstNum = 21;

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

        private string DealerHandStr => $"Dealer:{this.GetDealerHandsSum()}";

        private string PlayerHandStr => $"Player:{this.GetPlayerHandsSum()}";

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

            this.m_controller = new Dictionary<string, string>
            {
                {InputCommands.PlayerHand, this.DoPlayerHand() },
                {InputCommands.DealerHand, this.DoDealerHand() },
                {InputCommands.Help, this.DoHelp() },
                {InputCommands.Draw, this.DoDraw() },
                {InputCommands.End, this.DoEndGame() },
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

        private static int GetCardsNumberSum(IEnumerable<Card> hands)
        {
            // 現時点では単純な足し算をする。
            // 将来的にはAceを1or11として数える。
            return hands.Sum(x => x.Number);
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
            if (!this.m_controller.ContainsKey(input))
            {
                return "許されていないコマンドが入力されました。\nhelpを入力して、使い方を確認してください。";
            }

            return this.m_controller[input];
        }

        private readonly IDictionary<string, string> m_controller;

        public string DoPlayerHand()
        {
            return string.Join("\n", this.PlayerHands.Select(x => $"{x.Call}")) + $"\n{this.PlayerHandStr}";
        }

        public string DoDealerHand()
        {
            return this.DealerHands.FirstOrDefault()?.Call + $"\n{this.PlayerHandStr}";
        }

        public string DoHelp()
        {
            return $"使い方\n" +
                   $"{InputCommands.PlayerHand}:自分の手札を確認できます\n" +
                   $"{InputCommands.DealerHand}:ディーラーの手札を確認できます\n" +
                   $"{InputCommands.Draw}:手札を山札から引きます\n" +
                   $"{InputCommands.End}:終了して、ディーラーと勝負します";
        }

        public string DoDraw()
        {
            // プレイヤーが一枚ドロー
            this.PlayerDraw(1);

            if (this.GetPlayerHandsSum() <= m_burstNum)
            {
                // ドローしたカード表示
                return $"{this.PlayerHands.LastOrDefault()?.Suit}の{this.PlayerHands.LastOrDefault()?.Number}を引きました。\n{this.PlayerHandStr}";
            }

            // バーストしたらディーラーの勝利
            this.Winner = Actor.Dealer;
            return this.Burst(Actor.Player);

        }

        /// <summary>
        /// 最後にディーラーがドローする
        /// </summary>
        public string DoEndGame()
        {
            while (this.GetDealerHandsSum() < m_thresholdNum)
            {
                this.DealerDraw(1);
            }

            if (this.GetDealerHandsSum() > m_burstNum)
            {
                // バーストしたらプレイヤーの勝利
                this.Winner = Actor.Player;
                return this.Burst(Actor.Dealer);
            }

            // どちらもバーストしていないので、手札の数値で勝負
            this.Winner = this.GetPlayerHandsSum() > this.GetDealerHandsSum()
                ? Actor.Player
                : Actor.Dealer;

            return $"{this.PlayerHandStr}\n{this.DealerHandStr}";
        }

        private string Burst(Actor actor)
        {
            return $"{actor}がBurstしました\n{this.PlayerHandStr}\n{this.DealerHandStr}";
        }

        #endregion
    }

    public enum Actor
    {
        Player,
        Dealer,
    }
}
