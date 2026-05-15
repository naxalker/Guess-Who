using System;
using MessagePipe;
using UnityEngine;
using VContainer.Unity;

namespace GuessWho
{
    public class BoardController : IInitializable, IDisposable, IGameController
    {
        private const int TOTAL_CARDS = 24;
        private int _activeCardsCount;
        private IDisposable _subscription;

        private readonly ISubscriber<CardFlippedEvent> _cardFlippedSubscriber;
        private readonly IPublisher<ActiveCardsCountChangedEvent> _activeCardsCountChangedPublisher;

        public BoardController(ISubscriber<CardFlippedEvent> subscriber, IPublisher<ActiveCardsCountChangedEvent> publisher)
        {
            _cardFlippedSubscriber = subscriber;
            _activeCardsCountChangedPublisher = publisher;
        }

        public void Initialize()
        {
            _activeCardsCount = TOTAL_CARDS;
            _subscription = _cardFlippedSubscriber.Subscribe(CardFlippedHandler);
        }

        public void Dispose()
        {
            _subscription.Dispose();
        }

        private void CardFlippedHandler(CardFlippedEvent e)
        {
            _activeCardsCount = e.IsFlipped ? _activeCardsCount - 1 : _activeCardsCount + 1;
            _activeCardsCountChangedPublisher.Publish(new ActiveCardsCountChangedEvent { ActiveCardsCount = _activeCardsCount });
        }
    }
}