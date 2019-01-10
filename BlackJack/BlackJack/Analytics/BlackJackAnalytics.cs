using System;
using BlackJack.Model;

namespace BlackJack.Analytics
{
    public class BlackJackAnalytics
    {

        public void DoLoop()
        {
            for (var i = 1; i <= BlackJackCardController.BurstNum; i++)
            {
                Threshold(i);
            }

            for (var i = 4; i <= BlackJackCardController.BurstNum; i++)
            {
                Target(i);
            }
        }

        private static void Threshold(int val)
        {
            var playerWinCount = 0;
            const int loopCount = 10000;

            for (var j = 0; j < loopCount; j++)
            {
                var controller = new BlackJackCardController();

                while (!controller.IsGameEnd)
                {
                    controller.Input(controller.GetPlayerHandsSum() < val
                        ? BlackJackCardController.InputCommands.Draw
                        : BlackJackCardController.InputCommands.End);
                }

                if (controller.Winner == Actor.Player)
                {
                    playerWinCount++;
                }
            }

            Console.WriteLine($"{val}を超えたときに勝負しようとしてる人の勝率は、Player:{playerWinCount / (double)loopCount * 100}%、Dealer:{(loopCount - playerWinCount) / (double)loopCount * 100}%");
        }

        private static void Target(int val)
        {
            var playerWinCount = 0;
            const int loopCount = 100;
            var playCount = 0;

            while (playCount != loopCount)
            {
                var controller = new BlackJackCardController();

                if (controller.GetPlayerHandsSum() > val)
                {
                    // 手札が値より大きい場合、結果を無視する
                    continue;
                }

                // 目標の値まで引き続ける
                while (controller.GetPlayerHandsSum() < val)
                {
                    controller.Input(BlackJackCardController.InputCommands.Draw);
                }
                
                if (controller.GetPlayerHandsSum() != val)
                {
                    // 手札が値より大きい場合、結果を無視する
                    continue;
                }

                controller.Input(BlackJackCardController.InputCommands.End);
                
                if (controller.Winner == Actor.Player)
                {
                    playerWinCount++;
                }

                playCount++;
            }

            Console.WriteLine($"手札が{val}の時の勝率は、Player:{playerWinCount / (double)loopCount * 100}%、Dealer:{(loopCount - playerWinCount) / (double)loopCount * 100}%");
        }
    }
}
