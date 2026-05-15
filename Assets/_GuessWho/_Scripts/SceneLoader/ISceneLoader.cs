using Cysharp.Threading.Tasks;

namespace GuessWho
{
    public interface ISceneLoader
    {
        UniTask LoadScene(string sceneName);
    }
}
