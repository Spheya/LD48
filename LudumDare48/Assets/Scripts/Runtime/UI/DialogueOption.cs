using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

namespace LD48
{
    public class DialogueOption : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
    {
        public DialogueHandler Handler {get; set;}

        [SerializeField]
        protected TMPro.TextMeshProUGUI text;

        public void SetText(string content) => text.text = content;

        public void OnPointerEnter(PointerEventData eventData)
        {
            DialoguePointer.RotateTowards(transform as RectTransform);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Handler.Continue();
        }

    }
}