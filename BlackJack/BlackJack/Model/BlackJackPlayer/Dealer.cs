using System.Linq;

namespace BlackJack.Model.BlackJackPlayer
{
    public class Dealer : BlackJackPlayerBase
    {
        public override string Name => "Dealer";

        public string HandStr(bool isEnd = false)
        {
            var num = isEnd ? this.GetCardsNumberSum() : this.Hands.FirstOrDefault()?.Number;
            return $"Dealer:{num}";
        }

        public override string ShowHand()
        {
            return this.Hands.FirstOrDefault()?.Call + $"\n{this.HandStr()}\n";
        }
    }
}
