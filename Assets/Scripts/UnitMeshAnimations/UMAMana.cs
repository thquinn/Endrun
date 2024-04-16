using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UMAMana : MonoBehaviour
{
    public Vector3 bob;

    Vector3 initialPosition;
    float rate, time;

    void Start() {
        initialPosition = transform.localPosition;
        rate = Random.Range(.5f, .66f);
        time = Random.Range(0, rate);
    }

    void Update() {
        time += Time.deltaTime;
        transform.localPosition = initialPosition + Mathf.Sin(time * rate) * bob;
    }
}
