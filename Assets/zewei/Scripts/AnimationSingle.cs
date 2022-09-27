using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSingle : MonoBehaviour
{

    public enum AnimationType
    {
        Sin, Cos
    }

    public AnimationType _AnimationType;

    private float time;

    public float wave = 0.1f;
    private float y;
    public bool run = true;
    public float speed = 1;
    private void Start()
    {
        time = 0;
        y = transform.position.y;
    }

    void Update()
    {
        if (run)
        {
            time += Time.deltaTime * speed;
            Vector3 pos = transform.position;
            float _y = _AnimationType == AnimationType.Sin ? Mathf.Sin(time) : Mathf.Cos(time);
            _y = _y * wave + y;
            transform.position = new Vector3(pos.x, _y, pos.z);
        }
        
    }
}
