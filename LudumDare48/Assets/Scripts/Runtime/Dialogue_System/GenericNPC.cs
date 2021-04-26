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

        private void Start() 
        {
            dialogue.Reset();    
        }

        private void OnTriggerEnter2D(Collider2D other) 
        {
            handler.Init(dialogue);
            other.GetComponent<PlayerController>().SetPaused(true);
        }

        private void OnTriggerExit2D(Collider2D other)
            => print("On trigger exit 2d");
    }
}