using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UMAIcosahedron : MonoBehaviour
{
    static float SPEED = 20;

    Vector3 initialPosition;
    Vector3 axis;
    float dx, dy, dz;

    void Start() {
        initialPosition = transform.localPosition;
        float x = Random.Range(0, 2 * Mathf.PI);
        float y = Random.Range(0, 2 * Mathf.PI);
        float z = Random.Range(0, 2 * Mathf.PI);
        Vector3 axis = new Vector3(x, y, z);
        dx = Random.Range(1f, 2f) * Random.value < .5f ? -10 : 10;
        dy = Random.Range(1f, 2f) * Random.value < .5f ? -10 : 10;
        dz = Random.Range(1f, 2f) * Random.value < .5f ? -10 : 10;
    }

    void Update() {
        axis.x += dx * Time.deltaTime;
        axis.y += dy * Time.deltaTime;
        axis.z += dz * Time.deltaTime;
        transform.Rotate(axis, Time.deltaTime * SPEED);
    }
}
