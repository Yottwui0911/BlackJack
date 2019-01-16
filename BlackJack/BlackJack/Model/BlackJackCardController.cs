using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlackJack.Model.BlackJackPlayer;
using CardController.Model;

namespace BlackJack.Model
{
    public sealed class BlackJackCardController
    {
        public BlackJackCardController()
        {
            this.Dealer = new Dealer();
            this.Player = new User();
            this.Deck = new Deck();
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

        private BlackJackPlayerBase m_winner;

        /// <summary>
        /// 勝者
        /// </summary>
        public BlackJackPlayerBase Winner
        {
            get => this.m_winner;
            private set
            {
                this.m_winner = value;
                this.IsGameEnd = value != null;
            }
        }

        /// <summary>
        /// ゲーム終了かどうか
        /// </summary>
        public bool IsGameEnd { get; set; }

        /// <summary>
        /// ディーラー
        /// </summary>
        public Dealer Dealer { get; }

        /// <summary>
        /// プレイヤー
        /// </summary>
        public User Player { get; }

        /// <summary>
        /// 山札
        /// </summary>
        public Deck Deck { get; }

        #endregion

        #region Messages

        /// <summary>
        /// バーストしたときのメッセージ
        /// </summary>
        /// <param name="actor"></param>
        /// <returns></returns>
        private string BurstMsg(PlayerBase actor) => $"{actor?.Name}がBurstしました\n{this.Player.HandStr}\n{this.Dealer.HandStr(true)}\n";

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
        private static string DrawMsg(PlayerBase actor, Card card) => $"{actor?.Name}が{card?.Suit}の{card?.Number}を引きました。\n";

        #endregion

        #region methods

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            // ディーラーが2枚ドローする
            this.Dealer.Draw(this.Deck, 2);

            // プレイヤーが2枚ドローする
            this.Player.Draw(this.Deck, 2);
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
            return this.Player.ShowHand();
        }

        private string DoDealerHand()
        {
            return this.Dealer.ShowHand();
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
            this.Player.Draw(this.Deck, 1);
            var msg = DrawMsg(this.Player, this.Player.Hands.LastOrDefault());

            if (this.Player.GetCardsNumberSum() <= BurstNum)
            {
                // ドローしたカード表示
                return msg + $"{this.Player.HandStr}\n";
            }

            // バーストしたらディーラーの勝利
            this.Winner = this.Dealer;
            return msg + this.BurstMsg(this.Player);

        }

        /// <summary>
        /// 最後にディーラーがドローする
        /// </summary>
        private string DoEndGame()
        {
            var msg = new StringBuilder();
            while (this.Dealer.GetCardsNumberSum() < m_thresholdNum)
            {
                this.Dealer.Draw(this.Deck, 1);
                msg.Append(DrawMsg(this.Dealer, this.Dealer.Hands.LastOrDefault()));
            }

            if (this.Dealer.GetCardsNumberSum() > BurstNum)
            {
                // バーストしたらプレイヤーの勝利
                this.Winner = this.Player;
                msg.Append(this.BurstMsg(this.Dealer));
                return msg.ToString();
            }

            // どちらもバーストしていないので、手札の数値で勝負
            if (this.Player.GetCardsNumberSum() == this.Dealer.GetCardsNumberSum())
            {
                msg.Append("引き分けです！");
                this.IsGameEnd = true;
                return msg.ToString();
            }

            if (this.Player.GetCardsNumberSum() > this.Dealer.GetCardsNumberSum())
            {
                this.Winner = this.Player;
            }
            else
            {
                this.Winner = this.Dealer;
            }
            msg.Append($"{this.Player.HandStr}\n{this.Dealer.HandStr(true)}\n");
            return msg.ToString();
        }

        #endregion
    }
}
