using Colyseus.Schema;

namespace GuessWho
{
    public partial class Player : Schema
    {
        [Type(0, "string")]
        public string username = default;

        [Type(1, "number")]
        public float secretCardId = -1;

        [Type(2, "number")]
        public float cardsActive = 24;

        [Type(3, "boolean")]
        public bool isReady = default;
    }
}
