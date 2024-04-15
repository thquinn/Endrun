using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UMABrute : MonoBehaviour
{
    static float PERIOD = 2;

    float offset;
    float time;

    private void Start() {
        offset = Random.value;
    }

    void Update() {
        time += Time.deltaTime;
        float t = (time % PERIOD) / PERIOD;
        t = (t + offset) % 1;
        float theta = t * 2 * Mathf.PI;
        float wobble = Mathf.Sin(theta) * .5f + .5f;
        float xRot = Mathf.Lerp(-5, 5, wobble);
        transform.localRotation = Quaternion.Euler(xRot, 0, 0);
        float lift = Mathf.Sin(time * .35832f) * .5f + .5f;
        float yPos = Mathf.Lerp(-.05f, .05f, lift);
        Vector3 localPosition = transform.localPosition;
        localPosition.y = yPos;
        transform.localPosition = localPosition;
    }
}
