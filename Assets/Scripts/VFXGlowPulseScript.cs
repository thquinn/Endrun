using Assets.Code;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXGlowPulseScript : MonoBehaviour
{
    public float amount;

    Vector3 initialScale;
    float rate;
    float time;

    void Start() {
        initialScale = transform.localScale;
        rate = Random.Range(2f, 3f);
        time = Random.Range(0, rate);
    }

    void Update() {
        time += Time.deltaTime;
        float sin = Mathf.Sin(time * rate);
        float t = 1 - Mathf.Abs(sin);
        float s = Mathf.Lerp(1 - amount, 1 + amount, t);
        transform.localScale = Vector3.Scale(initialScale, new Vector3(s, s, 1));
    }
}
