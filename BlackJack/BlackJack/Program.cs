using System;
using BlackJack.Model;

namespace BlackJack
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var controller = new BlackJackCardController();
            Console.WriteLine("ようこそ。ブラックジャックへ。");

            while (!controller.IsGameEnd)
            {
                var input = Console.ReadLine();
                Console.WriteLine(controller.Input(input));
            }

            Console.WriteLine($"勝者は{controller.Winner}です。");
            Console.ReadKey();
        }
    }
}
