using UnityEngine;
using VContainer;

namespace GuessWho
{
    public class Preload : MonoBehaviour
    {
        [SerializeField] private string _startSceneName;

        private SceneLoader _sceneLoader;

        [Inject]
        private void Construct(SceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        private async void Awake()
        {
            await _sceneLoader.LoadScene(_startSceneName);
        }
    }
}
