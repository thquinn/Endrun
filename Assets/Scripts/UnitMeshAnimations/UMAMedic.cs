using Assets.Code;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UMAMedic : MonoBehaviour
{
    static float PERIOD = 4;

    List<Transform> spikes;
    float time;

    void Start()
    {
        spikes = new List<Transform>();
        foreach (Transform child in transform) {
            spikes.Add(child.GetChild(0));
        }
        Util.Shuffle(spikes);
    }

    void Update()
    {
        time += Time.deltaTime;
        for (int i = 0; i < spikes.Count; i++) {
            float t = time / PERIOD + i / 8f;
            t *= 2 * Mathf.PI;
            spikes[i].localPosition = new Vector3(0, .2f + Mathf.Sin(t) * .1f, 0);
        }
    }
}
