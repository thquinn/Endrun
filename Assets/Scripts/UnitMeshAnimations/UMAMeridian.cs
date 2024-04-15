using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UMAMeridian : MonoBehaviour
{
    static float PERIOD = 5f;

    float time;

    void Start() {
        time = Random.Range(0, PERIOD);
    }

    void Update() {
        time += Time.deltaTime;
        float theta = time / PERIOD * 2 * Mathf.PI;
        float sinScaled = Mathf.Sin(theta) * .5f + .5f;
        transform.localRotation = Quaternion.Euler(0, Mathf.Lerp(-170f, 170f, sinScaled), 0);
    }
}
