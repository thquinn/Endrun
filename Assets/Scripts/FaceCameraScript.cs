using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCameraScript : MonoBehaviour
{
    public bool flip;

    Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        Update();
    }

    // Update is called once per frame
    void Update()
    {
        if (flip) {
            transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position, cam.transform.up);
        } else {
            transform.LookAt(cam.transform, cam.transform.up);
        }
    }
}
