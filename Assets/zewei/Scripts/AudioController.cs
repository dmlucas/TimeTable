using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{

    public AudioSource clickAudio;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            clickAudio.Play();
        }
    }
}
