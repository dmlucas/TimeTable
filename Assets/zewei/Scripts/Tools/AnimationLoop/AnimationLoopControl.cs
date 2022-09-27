using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationLoopControl : MonoBehaviour
{

    public enum AnimationLoopType
    {
        alpha,
        scale,
        rotate,
        move,
    }

    public AnimationLoopType loopType = AnimationLoopType.alpha;

    public float minValue = 0.35f;

    public float maxValue = 1;
    [Header("tween direction")]
    public bool toMax = true;
    public float animSpeed = 3;
    //private void Start()
    //{

    //}
    void Update()
    {
        switch (loopType)
        {
            case AnimationLoopType.alpha:
                transform.SetAlphaProperty();
                float alpha = transform.GetAlphaValue();
                float targetValue = toMax ? maxValue : minValue;
                float lerpValue = Mathf.Lerp(alpha, targetValue, Time.deltaTime * animSpeed);
                transform.SetAlphaValue(lerpValue);
                if (Mathf.Abs(targetValue-lerpValue) < 0.05f)
                    toMax = !toMax;
                break;
            default:
                break;
        }
    }
}
