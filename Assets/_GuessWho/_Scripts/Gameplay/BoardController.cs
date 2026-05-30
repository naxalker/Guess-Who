using System;
using MessagePipe;
using UnityEngine;
using VContainer.Unity;

namespace GuessWho
{
    public class BoardController : IInitializable, IStartable, IDisposable, IGameController
    {
        private const int TOTAL_CARDS = 24;
        private int _activeCardsCount;
        private int _selectedCardId = -1;

        private readonly CardsSetConfig _cardsSetConfig;
        private readonly CardButton[] _cardButtons;
        private readonly IPublisher<ActiveCardsCountChangedEvent> _activeCardsCountChangedPublisher;

        public BoardController(CardsSetConfig cardsSetConfig, CardButton[] cardButtons, IPublisher<ActiveCardsCountChangedEvent> publisher)
        {
            _cardsSetConfig = cardsSetConfig;
            _cardButtons = cardButtons;
            _activeCardsCountChangedPublisher = publisher;
        }

        public int SelectedCardId
        {
            get => _selectedCardId;
            set
            {
                if (value < 0 || value >= TOTAL_CARDS)
                {
                    Debug.LogWarning($"Invalid card ID: {value}. It must be between 0 and {TOTAL_CARDS - 1}.");
                    return;
                }
                _selectedCardId = value;
            }
        }

        public void Initialize()
        {
            _activeCardsCount = TOTAL_CARDS;
        }

        public void Start()
        {
            InitializeCards();
        }

        public void Dispose()
        {
            for (int i = 0; i < _cardButtons.Length; i++)
            {
                _cardButtons[i].OnCardFlipped -= CardFlippedHandler;
            }
        }

        private void InitializeCards()
        {
            for (int i = 0; i < _cardButtons.Length; i++)
            {
                var character = _cardsSetConfig.Characters[i];
                _cardButtons[i].Initialize(character.Image, character.Name);
                _cardButtons[i].OnCardFlipped += CardFlippedHandler;
            }
        }

        private void CardFlippedHandler(bool isFlipped)
        {
            _activeCardsCount = isFlipped ? _activeCardsCount - 1 : _activeCardsCount + 1;
            _activeCardsCountChangedPublisher.Publish(new ActiveCardsCountChangedEvent { ActiveCardsCount = _activeCardsCount });
        }
    }
}