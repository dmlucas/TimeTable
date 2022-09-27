using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// record origin transform info
public class OriginTransform : MonoBehaviour
{
    public Vector3 localPosition;
    public Vector3 position;
    public Vector3 localScale;
    public Quaternion localRotation;
    public Quaternion rotation;
    public Vector3 localEulerAngles;
    public bool doLockAnim;
    public OriginTransform(Transform transform, bool doLockAnim = false)
    {
        localPosition = transform.localPosition;
        position = transform.position;
        localScale = transform.localScale;
        localRotation = transform.localRotation;
        rotation = transform.rotation;
        localEulerAngles = transform.localEulerAngles;
        this.doLockAnim = doLockAnim;
    }
}
