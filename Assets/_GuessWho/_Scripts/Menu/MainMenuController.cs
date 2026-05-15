using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace GuessWho
{
    public class MainMenuController : MonoBehaviour
    {
        private Button _createRoomButton;

        private MultiplayerController _multiplayerController;
        private SceneLoader _sceneLoader;

        [Inject]
        public void Construct(MultiplayerController multiplayerController, SceneLoader sceneLoader)
        {
            _multiplayerController = multiplayerController;
            _sceneLoader = sceneLoader;
        }

        private void Awake()
        {
            var uiDocument = GetComponent<UIDocument>();
            var root = uiDocument.rootVisualElement;

            _createRoomButton = root.Q<Button>("create-btn");

            _createRoomButton.clicked += CreateRoomButtonClickedHandler;
        }

        private void OnDestroy()
        {
            _createRoomButton.clicked -= CreateRoomButtonClickedHandler;
        }

        private async void CreateRoomButtonClickedHandler()
        {
            await _multiplayerController.ConnectToServerAsync();
            await _sceneLoader.LoadScene("Gameplay");
        }
    }
}
