using System;
using MessagePipe;
using TMPro;
using UnityEngine;
using VContainer;

namespace GuessWho
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class RemainedCardsText : MonoBehaviour
    {
        private TextMeshProUGUI _text;
        private ISubscriber<OpponentCardsChangedEvent> _subscriber;
        private IDisposable _subscription;

        [Inject]
        private void Construct(ISubscriber<OpponentCardsChangedEvent> subscriber)
        {
            _subscriber = subscriber;
        }

        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            _subscription = _subscriber.Subscribe(OnOpponentCardsChanged);
        }

        private void OnDisable()
        {
            _subscription?.Dispose();
        }

        private void OnOpponentCardsChanged(OpponentCardsChangedEvent e)
        {
            _text.text = e.ActiveCardsCount.ToString();
        }
    }
}