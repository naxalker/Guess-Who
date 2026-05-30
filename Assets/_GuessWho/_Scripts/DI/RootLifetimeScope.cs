using MessagePipe;
using VContainer;
using VContainer.Unity;

namespace GuessWho
{
    public class RootLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            var options = builder.RegisterMessagePipe();
            builder.RegisterMessageBroker<ActiveCardsCountChangedEvent>(options);
            builder.RegisterMessageBroker<OpponentCardsChangedEvent>(options);

            builder.Register<SceneLoader>(Lifetime.Singleton)
                    .AsSelf();

            builder.Register<MultiplayerController>(Lifetime.Singleton)
                   .AsImplementedInterfaces()
                   .AsSelf();
        }
    }
}
