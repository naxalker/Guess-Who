using System;
using System.Collections.Generic;
using UnityEngine;

namespace GuessWho
{
    [Serializable]
    public struct CharacterInfo
    {
        public string Index;
        public string Name;
        public Sprite Image;
    }

    [CreateAssetMenu(fileName = "CardsSetConfig", menuName = "GuessWho/CardsSetConfig")]
    public class CardsSetConfig : ScriptableObject
    {
        [SerializeField] private CharacterInfo[] _characters;

        public IReadOnlyList<CharacterInfo> Characters => _characters;
    }
}
