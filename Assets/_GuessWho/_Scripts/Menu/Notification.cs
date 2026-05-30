using UnityEngine;
using UnityEngine.UIElements;

namespace GuessWho
{
    public class Notification : MonoBehaviour
    {
        private Label _notificationText;
        private IVisualElementScheduledItem _hideTimer;

        private void Awake()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            _notificationText = root.Q<Label>("notification-text");
            Debug.Log(_notificationText);
        }

        public void Show(string text)
        {
            _notificationText.text = text;

            _hideTimer?.Pause();

            _notificationText.AddToClassList("notification--visible");

            _hideTimer = _notificationText.schedule
                .Execute(() => _notificationText.RemoveFromClassList("notification--visible"))
                .StartingIn(10000);
        }
    }
}
