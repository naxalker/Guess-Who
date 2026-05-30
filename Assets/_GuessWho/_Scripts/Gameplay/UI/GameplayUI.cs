using System;
using MessagePipe;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace GuessWho
{
    [RequireComponent(typeof(UIDocument))]
    public class GameplayUI : MonoBehaviour
    {
        private Label _label;
        private ISubscriber<OpponentCardsChangedEvent> _subscriber;
        private IDisposable _subscription;

        [Inject]
        private void Construct(ISubscriber<OpponentCardsChangedEvent> subscriber)
        {
            _subscriber = subscriber;
        }

        private void Awake()
        {
            _label = GetComponent<UIDocument>().rootVisualElement.Q<Label>("cards-amount-text");
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
            if (_label != null)
                _label.text = e.ActiveCardsCount.ToString();
        }
    }
}