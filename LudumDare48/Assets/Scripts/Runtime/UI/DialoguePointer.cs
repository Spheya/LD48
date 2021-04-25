using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace LD48
{
    public class DialoguePointer : MonoBehaviour
    {
        private static DialoguePointer instance;

        RectTransform self;

        private void Start() 
        {
            self = transform as RectTransform;    
            instance = this;
        }

        /**
        void Update()
        {
            var origionPosition = target.anchoredPosition;
            var targetSize = target.rect.width;

            var pointPosition = origionPosition;
            pointPosition.x -= targetSize * 0.5f;

            var direction = pointPosition - self.anchoredPosition;

            self.localEulerAngles = new Vector3(0,0, Vector2.SignedAngle(Vector2.right, direction));
        }**/

        public static void RotateTowards(RectTransform target) => instance._RotateTowards(target);

        private void _RotateTowards(RectTransform pointerTarget)
        {
            Vector2 origin = pointerTarget.anchoredPosition;
            float width = pointerTarget.rect.width;

            Vector2 pointTowardPosition = origin;
            pointTowardPosition.x -= width * 0.5f;

            Vector2 direction = pointTowardPosition - self.anchoredPosition;

            self.DOLocalRotate(endValue: new Vector3(0, 0, Vector2.SignedAngle(Vector2.right, direction)), duration: 0.2f);
        }
    }
}