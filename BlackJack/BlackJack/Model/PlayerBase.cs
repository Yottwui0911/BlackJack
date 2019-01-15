using System.Collections.Generic;

namespace BlackJack.Model
{
    public abstract class PlayerBase
    {
        public virtual string Name { get; } = string.Empty;

        public List<Card> Hands { get; set; } = new List<Card>();
    }
}
