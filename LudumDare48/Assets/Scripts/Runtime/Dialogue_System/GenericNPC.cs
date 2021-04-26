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
                handler.Init(dialogue);
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
    }
}