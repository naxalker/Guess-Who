using Cysharp.Threading.Tasks;

namespace GuessWho
{
    public interface IMultiplayerService
    {
        UniTask ConnectToServerAsync();
    }
}
