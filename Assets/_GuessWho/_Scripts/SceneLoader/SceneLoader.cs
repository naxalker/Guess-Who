using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace GuessWho
{
    public class SceneLoader : ISceneLoader
    {
        public async UniTask LoadScene(string sceneName)
        {
            await SceneManager.LoadSceneAsync(sceneName).ToUniTask();
        }
    }
}
