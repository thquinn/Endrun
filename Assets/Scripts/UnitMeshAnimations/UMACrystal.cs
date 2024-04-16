using Assets.Code;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UMACrystal : MonoBehaviour
{
    float xT, yT;

    void Start() {
        xT = Random.Range(0f, 10f);
        yT = Random.Range(0f, 10f);
    }

    void Update() {
        xT += Time.deltaTime;
        yT += Time.deltaTime;
        float xRot = Mathf.Lerp(-10, 40, Util.NormalizedSin(xT * .33f));
        float yRot = yT * 40;
        transform.localRotation = Quaternion.Euler(xRot, yRot, 0);
    }
}
