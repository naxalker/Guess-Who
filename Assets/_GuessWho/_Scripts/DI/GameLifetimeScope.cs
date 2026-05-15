using VContainer;
using VContainer.Unity;

namespace GuessWho
{
    public class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<BoardController>(Lifetime.Singleton)
                   .AsImplementedInterfaces();
        }
    }
}