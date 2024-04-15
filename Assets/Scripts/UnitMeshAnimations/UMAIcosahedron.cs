using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UMAIcosahedron : MonoBehaviour
{
    static float SPEED = 40;

    Vector3 axis;
    float dx, dy, dz;

    void Start() {
        float x = Random.Range(0, 2 * Mathf.PI);
        float y = Random.Range(0, 2 * Mathf.PI);
        float z = Random.Range(0, 2 * Mathf.PI);
        axis = new Vector3(x, y, z);
        dx = Random.Range(1f, 2f) * Random.value < .5f ? -20 : 20;
        dy = Random.Range(1f, 2f) * Random.value < .5f ? -20 : 20;
        dz = Random.Range(1f, 2f) * Random.value < .5f ? -20 : 20;
        transform.localRotation = Random.rotationUniform;
    }

    void Update() {
        axis.x += dx * Time.deltaTime;
        axis.y += dy * Time.deltaTime;
        axis.z += dz * Time.deltaTime;
        transform.Rotate(axis, Time.deltaTime * SPEED);
    }
}
