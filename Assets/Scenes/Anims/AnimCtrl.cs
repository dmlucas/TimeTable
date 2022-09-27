using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimCtrl : MonoBehaviour
{
    Animator _anim;
    void Start()
    {
        _anim = GetComponent<Animator>();
        
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)){
            _anim.SetInteger("IntTrigger", 0);
        }
        if (Input.GetKeyDown(KeyCode.S)){
            _anim.SetInteger("IntTrigger", 1);
        }
        if (Input.GetKeyDown(KeyCode.D)){
            _anim.SetInteger("IntTrigger", 2);
        }
    }
}
