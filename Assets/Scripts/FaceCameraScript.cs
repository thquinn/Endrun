using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCameraScript : MonoBehaviour
{
    Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(cam.transform.position, Vector3.up);
    }
}
