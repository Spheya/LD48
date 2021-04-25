using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD48
{
    [CreateAssetMenu(menuName = "Scripted/Dialogue", fileName = "New Dialogue")]
    public class Dialogue : ScriptableObject
    {
        [SerializeField]
        DialogueSnippet[] snippets;
        private int currentSnippet = -1;

        [SerializeField]
        private string endOfDialogueRepeatable;

        public string OutOfDialogueText => endOfDialogueRepeatable;

        ///<summary>returns false when it hits the end, snippet is given through the out</summary>
        public bool Advance(out DialogueSnippet snippet)
        {
            currentSnippet++;
            snippet = snippets[Mathf.Clamp(currentSnippet, 0, snippets.Length-1)];
            return currentSnippet < snippets.Length;
        }

        public void Reset() 
        {
            currentSnippet = -1;
        }

        [System.Serializable]
        public class DialogueSnippet 
        {
            public string npcText;
            [Tooltip("The replies the player can give, leave empty to just have the NPC talk.")]
            public string[] playerReplies;
        }
    }
}