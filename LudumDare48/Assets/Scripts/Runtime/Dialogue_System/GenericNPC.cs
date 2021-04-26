using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD48
{
    public class GenericNPC : MonoBehaviour
    {
        [SerializeField]
        private Dialogue dialogue;
        [SerializeField]
        private DialogueHandler handler;
        [SerializeField]
        private GameObject speechBubble;
        [SerializeField]
        private AudioSource audioSource;
        [SerializeField]
        private AudioClip voice;
        [SerializeField]
        private UnityEngine.Events.UnityEvent onStartDialogue, onDialogueComplete;

        private bool dialogueDoneAlready = false;
        private bool playerInRange = false;
        private PlayerController player;

        private void Start() 
        {
            dialogue.Reset();    
        }

        private void Update() 
        {
            if(playerInRange && Input.GetKeyDown(KeyCode.E)) //TODO: display visual cue
            {
                speechBubble.SetActive(false);
                player.SetPaused(true);
                onStartDialogue?.Invoke();
                if(!dialogueDoneAlready)
                    DialogueHandler.OnDialogueEnd += DialogueEnded;
                handler.Init(dialogue, audioSource, voice);
            }
        }

        private void OnTriggerEnter2D(Collider2D other) 
        {
            player = other.GetComponent<PlayerController>();
            playerInRange = player;
            speechBubble.SetActive(playerInRange);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if(playerInRange)
            {
                player = null;
                playerInRange = false;
                speechBubble.SetActive(false);
            }
        }

        private void DialogueEnded()
        {
            if(!dialogueDoneAlready)
            {
                dialogueDoneAlready = true;
                onDialogueComplete?.Invoke();
                DialogueHandler.OnDialogueEnd -= DialogueEnded;
            }
        }
    }
}