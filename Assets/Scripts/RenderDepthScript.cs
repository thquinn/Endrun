using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RenderDepthScript : MonoBehaviour
{
    void OnEnable() {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.DepthNormals;
    }
}
