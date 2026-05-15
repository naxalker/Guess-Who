using Colyseus.Schema;

namespace GuessWho
{
    public partial class GuessWhoState : Schema
    {
        [Type(0, "map", typeof(MapSchema<Player>))]
        public MapSchema<Player> players = new MapSchema<Player>();

        [Type(1, "string")]
        public string status = "waiting";

        [Type(2, "string")]
        public string currentTurn = default;

        [Type(3, "string")]
        public string winner = default;
    }
}