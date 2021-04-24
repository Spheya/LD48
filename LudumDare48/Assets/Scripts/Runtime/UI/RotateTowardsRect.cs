using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowardsRect : MonoBehaviour
{
    public RectTransform target;

    RectTransform self;

    private void Start() 
    {
        self = transform as RectTransform;    
    }

    void Update()
    {
        var origionPosition = target.anchoredPosition;
        var targetSize = target.rect.width;

        var pointPosition = origionPosition;
        pointPosition.x -= targetSize * 0.5f;

        var direction = pointPosition - self.anchoredPosition;

        self.localEulerAngles = new Vector3(0,0, Vector2.SignedAngle(Vector2.right, direction));
    }
}
