using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UMAWarrior : MonoBehaviour
{
    static float PERIOD = 2;

    public float offset;

    float time;

    void Update() {
        time += Time.deltaTime;
        float t = (time % PERIOD) / PERIOD;
        t = (t + offset) % 1;
        float theta = t * 2 * Mathf.PI;
        float wobble = Mathf.Sin(theta) * .5f + .5f;
        float xRot = Mathf.Lerp(-10, 40, wobble);
        transform.localRotation = Quaternion.Euler(xRot, 0, 0);
        float lift = wobble;
        float yPos = Mathf.Lerp(.1f, -.1f, lift);
        Vector3 localPosition = transform.localPosition;
        localPosition.y = yPos;
        transform.localPosition = localPosition;
    }
}
