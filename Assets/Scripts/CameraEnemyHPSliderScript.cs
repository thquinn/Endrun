using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEnemyHPSliderScript : MonoBehaviour
{
    public float factor;

    void Update() {
        float dot = Vector3.Dot(transform.forward, Vector3.up);
        transform.localPosition = new Vector3(0, dot * factor, 0);
    }
}
