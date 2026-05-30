using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace GuessWho
{
    [RequireComponent(typeof(UIDocument))]
    public class PreparingPhaseUI : MonoBehaviour
    {
        private (Button button, int index) _selectedCharacter;
        private Label _selectedCharacterLabel;

        private CardsSetConfig _cardsSetConfig;

        [Inject]
        private void Construct(CardsSetConfig cardsSetConfig)
        {
            _cardsSetConfig = cardsSetConfig;
        }

        private void Awake()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            var characterButtons = root.Query<Button>(className: "character-btn").ToList();
            for (int i = 0; i < characterButtons.Count; i++)
            {
                int index = i;
                characterButtons[i].Q<Label>().text = _cardsSetConfig.Characters[i].Name;
                characterButtons[i].Q<Image>().sprite = _cardsSetConfig.Characters[i].Image;
                characterButtons[i].clicked += () => CharacterButtonClickedHandler(characterButtons[index], index);
            }

            _selectedCharacterLabel = root.Q<Label>("selected-character-label");
            _selectedCharacterLabel.text = "SELECT YOUR SECRET CHARACTER";
        }

        private void CharacterButtonClickedHandler(Button button, int index)
        {
            _selectedCharacter.button?.RemoveFromClassList("character-btn--selected");
            button.AddToClassList("character-btn--selected");
            _selectedCharacter = (button, index);

            _selectedCharacterLabel.text = $"YOUR SECRET: {_cardsSetConfig.Characters[index].Name.ToUpper()}";
        }
    }
}
