using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace GuessWho
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private CardsSetConfig _cardsSetConfig;
        [SerializeField] private CardButton[] _cardButtons;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<BoardController>(Lifetime.Singleton)
                   .AsImplementedInterfaces()
                   .WithParameter(_cardButtons);

            builder.RegisterInstance(_cardsSetConfig).AsSelf();
        }
    }
}