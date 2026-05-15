using System;
using UnityEngine;

namespace GuessWho
{
    [Serializable]
    public class ChatMessage
    {
        public string senderId;
        public string text;
    }

    public class Chat : MonoBehaviour
    {
    }
}