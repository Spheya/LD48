using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace LD48
{
    public class DialogueHandler : MonoBehaviour
    {
        [SerializeField]
        private DialogueOption[] options;
        [SerializeField]
        private float distance = 400f, angleOffset = 15f;

        [SerializeField]
        private TextMeshProUGUI npcText;
        [SerializeField]
        private CanvasGroup npcGroup;
        [SerializeField, Tooltip("how many characters per second?")]
        private float textSpeed = 10;
        [SerializeField]
        private CanvasGroup optionsGroup;

        //the currently active dialogue object.
        [SerializeField]
        Dialogue dialogue;

        // Start is called before the first frame update
        void Start()
        {
            for(int i = 0; i < options.Length; i++)
            {
                options[i].Handler = this;
                options[i].gameObject.SetActive(false);
            }
            dialogue.Reset();
            Continue();
        }

        public void Init(Dialogue dia)
        {
            dialogue = dia;
            if(!dialogue.Advance(out Dialogue.DialogueSnippet snippet))
            {
                //already finished dialogue with this character.
                DoDialogueEndSequence();
            }
            else 
                ShowDialogue(snippet);
        }

        //The player has clicked an option and the dialogue should now continue.
        //Which option is clicked is completely irrelevant rn
        public void Continue()
        {
            //advance the dialogue to the next bit.
            if(!dialogue.Advance(out Dialogue.DialogueSnippet snippet))
            { 
                //TODO: handle the dialogue ending.
                npcGroup.DOFade(0, 1).OnComplete(() => gameObject.SetActive(false)).PlayForward(); //TODO: OnComplete => free player so they can move again.
                //gameObject.SetActive(false);
                return;
            }

            ShowDialogue(snippet);
        }

        private void DoDialogueEndSequence()
        {
            var sequence = DOTween.Sequence();
            //start by displaying the text.
            float textDuration = (float) dialogue.OutOfDialogueText.Length / textSpeed;
            sequence.Append(DOTween.To(() => npcText.text, x => npcText.text = x, dialogue.OutOfDialogueText, textDuration));

            //after a 2s delay, fade out what the npc is saying.
            sequence.Append(npcGroup.DOFade(0, 1).SetDelay(2f)); //TODO: OnComplete => free player so they can move again.

            //start the sequence.
            sequence.PlayForward();
        }

        private void ShowDialogue(Dialogue.DialogueSnippet snippet)
        {
            //clear the npc chat box.
            npcText.text = "";

            //create the sequence.
            var sequence = DOTween.Sequence();
            //1. Fade out all the options.
            sequence.Append(optionsGroup.DOFade(endValue: 0.0f, duration: 0.5f)); //.OnComplete(() => source.Play())
            //2. show the new text, then set up the options.
            float textDuration = (float) snippet.npcText.Length / textSpeed;
            sequence.Append(DOTween.To(() => npcText.text, x => npcText.text = x, snippet.npcText, textDuration).OnComplete(() => SetupOptions(snippet)));
            //3. fade the options back in if necessary
            if(snippet.playerReplies.Length > 0)
                sequence.Append(optionsGroup.DOFade(endValue: 1.0f, duration: 0.5f).SetDelay(0.5f));
            //4. start the sequence.
            sequence.PlayForward();
        }

        private void SetupOptions(Dialogue.DialogueSnippet snippet)
        {
            int i = 0;
            int replyCount = snippet.playerReplies.Length;
            if(replyCount == 0)
            {
                //hide all options.
                for(; i < options.Length; i++)
                    options[i].gameObject.SetActive(false);

                //Wait for a short delay, then wait for player input
                StartCoroutine(WaitForKeyPress());
                IEnumerator WaitForKeyPress()
                {
                    yield return new WaitForSeconds(1f);
                    yield return new WaitUntil(() => Input.anyKeyDown);
                    Continue();
                }
                return;
            }
            //the total spread of the options in degrees.
            float spread = ((float)replyCount - 1f) * angleOffset;
            //start at half the negative spread.
            float angle = spread * -0.5f;
            for(; i < replyCount; i++)
            {
                options[i].gameObject.SetActive(true);
                options[i].SetText(snippet.playerReplies[i]);
                
                //place the option.
                Vector2 baseOffset = new Vector2(distance, 0);
                Vector2 rotatedOffset = Quaternion.AngleAxis(angle, Vector3.forward) * baseOffset;

                options[i].GetComponent<RectTransform>().anchoredPosition = rotatedOffset;

                //increment the angle.
                angle += angleOffset;
            }
            for(; i < options.Length; i++)
            {
                options[i].gameObject.SetActive(false);
            }
        }

    }
}