using System.Linq;

namespace BlackJack.Model.BlackJackPlayer
{
    public class User : BlackJackPlayerBase
    {
        public override string Name => "Player";

        public string HandStr => $"Player:{this.GetCardsNumberSum()}";

        public override string ShowHand()
        {
            return string.Join("\n", this.Hands.Select(x => $"{x.Call}")) + $"\n{this.HandStr}\n";
        }
    }
}
