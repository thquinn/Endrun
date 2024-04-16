using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UMAFlatSpin : MonoBehaviour
{
    public Vector3 eulersPerSecond;

    void Update() {
        transform.Rotate(eulersPerSecond * Time.deltaTime);
    }
}
