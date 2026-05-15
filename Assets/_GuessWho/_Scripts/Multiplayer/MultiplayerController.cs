using System;
using System.Collections.Generic;
using System.Threading;
using Colyseus;
using Colyseus.Schema;
using Cysharp.Threading.Tasks;
using MessagePipe;
using UnityEngine;
using VContainer.Unity;

namespace GuessWho
{
    public class MultiplayerController : IInitializable, IDisposable, IMultiplayerService
    {
        private const string ServerUrl = "ws://localhost:2567";

        private readonly ISubscriber<ActiveCardsCountChangedEvent> _activeCardsCountChangedSubscriber;
        private readonly IPublisher<OpponentCardsChangedEvent> _opponentCardsPublisher;

        private Client _client;
        private Room<GuessWhoState> _room;
        private CancellationTokenSource _cts;
        private IDisposable _subscription;

        public MultiplayerController(
            ISubscriber<ActiveCardsCountChangedEvent> activeCardsCountChangedSubscriber,
            IPublisher<OpponentCardsChangedEvent> opponentCardsPublisher)
        {
            _activeCardsCountChangedSubscriber = activeCardsCountChangedSubscriber;
            _opponentCardsPublisher = opponentCardsPublisher;
        }

        public void Initialize()
        {
            _cts = new CancellationTokenSource();
            _subscription = _activeCardsCountChangedSubscriber.Subscribe(ActiveCardsCountChangedHandler);
        }

        public void Dispose()
        {
            _subscription?.Dispose();
            _cts?.Cancel();
            _cts?.Dispose();
            _room?.Leave();
        }


        public async UniTask ConnectToServerAsync()
        {
            _client = new Client(ServerUrl);
            try
            {
                var options = new Dictionary<string, object>
                {
                    { "username", $"Player_{UnityEngine.Random.Range(100, 999)}" }
                };

                _room = await _client.Create<GuessWhoState>("guess_who", options)
                    .AsUniTask()
                    .AttachExternalCancellation(_cts.Token);

                Debug.Log($"Entered room: {_room.RoomId}");
                SetupStateHandlers();
            }
            catch (Exception e)
            {
                Debug.LogError($"Connection error: {e.Message}");
            }
        }

        public async UniTask SendChatMessage(string text)
        {
            if (_room == null) return;
            await _room.Send("chat_message", new { text }).AsUniTask();
        }

        public async UniTask EndTurn()
        {
            if (_room == null) return;
            await _room.Send("end_turn").AsUniTask();
        }

        private void SetupStateHandlers()
        {
            var callbacks = Callbacks.Get(_room);

            callbacks.OnAdd(state => state.players, (sessionId, player) =>
            {
                Debug.Log($"Player joined: {player.username}");

                callbacks.Listen(player, p => p.cardsActive, (current, previous) =>
                {
                    if (sessionId != _room.SessionId)
                    {
                        _opponentCardsPublisher.Publish(new OpponentCardsChangedEvent
                        {
                            ActiveCardsCount = (int)current
                        });
                    }
                });
            });

            callbacks.OnRemove(state => state.players, (sessionId, player) =>
            {
                Debug.Log($"Player left: {sessionId}");
            });

            callbacks.Listen(state => state.currentTurn, (current, previous) =>
            {
                Debug.Log($"Turn changed: {previous} -> {current}");
            });

            _room.OnMessage<ChatMessage>("chat_message", message =>
            {
                Debug.Log($"[{message.senderId}]: {message.text}");
            });
        }

        private void ActiveCardsCountChangedHandler(ActiveCardsCountChangedEvent e)
        {
            SendActiveCardsCount(e.ActiveCardsCount).Forget();
        }

        private async UniTaskVoid SendActiveCardsCount(int count)
        {
            if (_room == null) return;
            await _room.Send("update_active_cards", new { count }).AsUniTask();
        }
    }
}