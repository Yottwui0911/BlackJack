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
    }
}
