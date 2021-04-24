using UnityEngine;
using UnityEngine.EventSystems;

public class HoverRotate : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField]
    RotateTowardsRect rotator;

    public void OnPointerEnter(PointerEventData eventData)
    {
        rotator.target = this.transform as RectTransform;
    }

}
