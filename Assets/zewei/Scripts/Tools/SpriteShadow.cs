using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteAlways]
public class SpriteShadow : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {
        GetComponent<SpriteRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        GetComponent<SpriteRenderer>().receiveShadows = true;

    }
}
