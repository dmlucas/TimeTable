using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ClickSelectMove : MonoBehaviour
{
    public GameObject selector;
    public Camera _Camera;
    public GameObject desktop;
    void Start()
    {
        if (_Camera == null)
            _Camera = Camera.main;
        if (selector == null)
            selector = gameObject;
        selector.AddComponent<OutlineModel>();
        selector.GetComponent<OutlineModel>().enabled = false;
        desktop = GameObject.Find("Desktop");
    }

    private Vector3 pos;
    private Vector3 modelPos;

    private bool clickSelDown = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(_Camera.ScreenPointToRay(Input.mousePosition), out RaycastHit raycastHit))
            {
                if (raycastHit.transform.Equals(selector.transform))
                {
                    selector.GetComponent<OutlineModel>().enabled = true;
                    var raycastAll = Physics.RaycastAll(_Camera.ScreenPointToRay(Input.mousePosition), 200);
                    foreach (var VARIABLE in raycastAll)
                    {
                        if (VARIABLE.collider.transform.name.Contains("Desktop"))
                        {
                            pos = VARIABLE.point;
                            modelPos = transform.position;
                            clickSelDown = true;
                            break;
                        }
                    }
                }
                else
                {
                    selector.GetComponent<OutlineModel>().enabled = false;
                }
            }
            else
            {
                selector.GetComponent<OutlineModel>().enabled = false;
            }
        }

        if (selector.GetComponent<OutlineModel>().enabled && Input.GetMouseButton(0))
        {
            var raycastAll = Physics.RaycastAll(_Camera.ScreenPointToRay(Input.mousePosition), 200);
            foreach (var VARIABLE in raycastAll)
            {
                if (VARIABLE.collider.transform.name.Contains("Desktop"))
                {
                    var _pos = VARIABLE.point - pos;
                    transform.position = modelPos + new Vector3(_pos.x, 0, _pos.z);
                    break;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            selector.GetComponent<OutlineModel>().enabled = false;
            if (Physics.Raycast(_Camera.ScreenPointToRay(Input.mousePosition), out RaycastHit raycastHit))
            {
                if (raycastHit.transform.Equals(selector.transform))
                {
                    if (clickSelDown)
                    {
                        // complete click
                        FindToggleAnim();
                    }
                }
            }

            clickSelDown = false;
        }

    }

    public void FindToggleAnim()
    {
        var anim = transform.GetComponentInChildren<Animator>(true);
        // if (anim.gameObject.activeSelf)
        // {
        //     // anim.speed = -1;
        //     anim.Play("New Animation2");
        // }
        // else
        // {
        //     anim.gameObject.SetActive(true);
        //     // anim.speed = 1;
        //     anim.Play("New Animation2");
        // }
        anim.gameObject.SetActive(true);
        // anim.speed = 1;
        anim.Play("New Animation2");
        // Debug.Log("Speed = " + anim.GetFloat("Speed"));
        // anim.gameObject.SetActive(true);
        // anim.SetFloat("Speed", anim.GetFloat("Speed") > 0 ? -1 : 1);
    }

    // find anim and play
    public void FindPlayAnim()
    {
        var anim = transform.GetComponentInChildren<Animator>(true);
        anim.gameObject.SetActive(true);
        anim.SetFloat("Speed", 1);
    }
    public void FindStopAnim()
    {
        var anim = transform.GetComponentInChildren<Animator>(true);
        anim.gameObject.SetActive(true);
        anim.SetFloat("Speed", -1);
    }

    private float factor = 0.85f;
    private void FixedUpdate()
    {
        var bounds = desktop.GetComponent<MeshRenderer>().bounds;
        var pos = transform.position;
        if (pos.x < (-bounds.center.x + bounds.min.x) * factor + bounds.center.x)
        {
            transform.position = new Vector3((-bounds.center.x + bounds.min.x) * factor + bounds.center.x * factor, pos.y, pos.z);
        }
        if (pos.x > (-bounds.center.x + bounds.max.x) * factor + bounds.center.x)
        {
            transform.position = new Vector3((-bounds.center.x + bounds.max.x) * factor + bounds.center.x, pos.y, pos.z);
        }
        if (pos.z < (-bounds.center.z + bounds.min.z) * factor + bounds.center.z)
        {
            transform.position = new Vector3(pos.x, pos.y, (-bounds.center.z + bounds.min.z) * factor + bounds.center.z);
        }
        if (pos.z > (-bounds.center.z + bounds.max.z) * factor + bounds.center.z)
        {
            transform.position = new Vector3(pos.x, pos.y, (-bounds.center.z + bounds.max.z) * factor + bounds.center.z);
        }
    }
}
