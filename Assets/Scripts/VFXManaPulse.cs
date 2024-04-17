using Assets.Code;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManaPulse : MonoBehaviour
{
    public GameObject prefabPulse;
    public float rate;

    Vector3 chunksNormal;
    float time;

    void Update() {
        if (chunksNormal == Vector3.zero) {
            chunksNormal = NavMeshUtil.GetChunksNormal(transform.position);
            if (chunksNormal == Vector3.zero) {
                return;
            }
        }
        time += Time.deltaTime;
        if (time > rate) {
            Transform pulse = Instantiate(prefabPulse).transform;
            pulse.position = transform.position;
            pulse.localRotation = Quaternion.LookRotation(chunksNormal);
            time -= rate;
        }
    }
}
