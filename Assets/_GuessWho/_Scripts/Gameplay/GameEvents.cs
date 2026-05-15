namespace GuessWho
{
    public struct CardFlippedEvent
    {
        public bool IsFlipped;
    }

    public struct ActiveCardsCountChangedEvent
    {
        public int ActiveCardsCount;
    }

    public struct OpponentCardsChangedEvent
    {
        public int ActiveCardsCount;
    }
}