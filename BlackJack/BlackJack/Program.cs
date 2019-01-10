using System;
using BlackJack.Analytics;
using BlackJack.Model;

namespace BlackJack
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var controller = new BlackJackCardController();
            Console.WriteLine("ようこそ。ブラックジャックへ。");
            Console.WriteLine(controller.Input(BlackJackCardController.InputCommands.Help));

            while (!controller.IsGameEnd)
            {
                var input = Console.ReadLine();
                if (input == "analytics")
                {
                    Analytics();
                }
                else
                {
                    Console.WriteLine(controller.Input(input));
                }
            }

            Console.WriteLine($"勝者は{controller.Winner}です。");
            Console.ReadKey();
        }
        
        public static void Analytics()
        {
            var an = new BlackJackAnalytics();
            an.DoLoop();
        }
    }
}
