using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteAlways]
public class CameraParameterTest : MonoBehaviour
{
    public new  Camera camera;
    void Start()
    {
        if (camera == null)
            camera = Camera.main;
    }

    void Update()
    {
        var camPos = camera.transform.position;
        var curPos = transform.position;
        var lookAtPos = new Vector3(camPos.x, curPos.y, camPos.z);
        transform.LookAt(lookAtPos);
        transform.Rotate(Vector3.up * 180, Space.World);
    }
}
