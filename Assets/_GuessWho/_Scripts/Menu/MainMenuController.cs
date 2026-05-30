using System;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace GuessWho
{
    [RequireComponent(typeof(Notification))]
    public class MainMenuController : MonoBehaviour
    {
        private Button _createRoomButton;
        private TextField _roomCodeTextField;
        private Button _joinButton;

        private Notification _notification;
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
            _notification = GetComponent<Notification>();

            var uiDocument = GetComponent<UIDocument>();
            var root = uiDocument.rootVisualElement;

            _roomCodeTextField = root.Q<TextField>("join-text-field");
            _joinButton = root.Q<Button>("join-btn");
            _createRoomButton = root.Q<Button>("create-btn");

            _joinButton.SetEnabled(false);

            _roomCodeTextField.RegisterValueChangedCallback(TextFieldValueChangedHandler);
            _joinButton.clicked += JoinRoomButtonClickedHandler;
            _createRoomButton.clicked += CreateRoomButtonClickedHandler;
        }

        private void OnDestroy()
        {
            _roomCodeTextField.UnregisterValueChangedCallback(TextFieldValueChangedHandler);
            _joinButton.clicked -= JoinRoomButtonClickedHandler;
            _createRoomButton.clicked -= CreateRoomButtonClickedHandler;
        }

        private void TextFieldValueChangedHandler(ChangeEvent<string> evt)
        {
            string filtered = FilterInput(evt.newValue);

            if (_roomCodeTextField.value != filtered)
            {
                _roomCodeTextField.SetValueWithoutNotify(filtered);
            }

            _joinButton.SetEnabled(filtered.Length == _roomCodeTextField.maxLength);
        }

        private async void JoinRoomButtonClickedHandler()
        {
            _joinButton.SetEnabled(false);

            _notification.Show("Joining room...");
            if (await _multiplayerController.JoinRoomAsync(_roomCodeTextField.value))
            {
                _notification.Show("Room joined! Loading game...");
                await _sceneLoader.LoadScene("Gameplay");
            }
            else
            {
                _notification.Show("Failed to join room. Please check the code and try again.");
            }

            _joinButton.SetEnabled(_roomCodeTextField.text.Length == _roomCodeTextField.maxLength);
        }

        private async void CreateRoomButtonClickedHandler()
        {
            _createRoomButton.SetEnabled(false);

            _notification.Show("Creating room...");
            if (await _multiplayerController.CreateRoomAsync())
            {
                _notification.Show("Room created! Loading game...");
                await _sceneLoader.LoadScene("Gameplay");
            }
            else
            {
                _notification.Show("Failed to create room. Please try again.");
            }

            _createRoomButton.SetEnabled(true);
        }

        private string FilterInput(string value)
        {
            value = value.ToUpper();

            StringBuilder builder = new();

            foreach (char c in value)
            {
                if (c >= 'A' && c <= 'Z' || char.IsDigit(c))
                {
                    builder.Append(c);
                }
            }

            return builder.ToString();
        }
    }
}
