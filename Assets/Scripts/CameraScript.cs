using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    int chunksLayer;
    Vector3 chunksCenter, vCenter;
    float distance;
    public float minDistance, maxDistance, sensitivity, scrollSensitivity;
    float horizontalAngle = Mathf.PI * 2 / 3;
    float verticalAngle = Mathf.PI / 6;
    bool firstInput;

    void Start() {
        chunksLayer = LayerMask.NameToLayer("Chunks");
        chunksCenter = GetChunksCenter();
    }

    void Update() {
        chunksCenter = Vector3.SmoothDamp(chunksCenter, GetChunksCenter(), ref vCenter, 1);

        // Input.
        distance *= Mathf.Pow(scrollSensitivity, Input.mouseScrollDelta.y);
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
        if (Input.GetMouseButton(2)) {
            float mx = Input.GetAxis("Mouse X");
            float my = Input.GetAxis("Mouse Y");
            if (!firstInput) {
                firstInput = true;
            } else {
                horizontalAngle -= mx * sensitivity;
                verticalAngle -= my * sensitivity;
                verticalAngle = Mathf.Clamp(verticalAngle, Mathf.PI * .1f, Mathf.PI * .49f);
            }
        }

        // Set position.
        float xzDistance = distance * Mathf.Cos(verticalAngle);
        float x = Mathf.Cos(horizontalAngle) * xzDistance;
        float y = Mathf.Sin(verticalAngle) * distance;
        float z = Mathf.Sin(horizontalAngle) * xzDistance;
        transform.localPosition = new Vector3(x, y, z);
        transform.LookAt(chunksCenter);
    }

    Vector3 GetChunksCenter() {
        Bounds bounds = new Bounds();
        foreach (GameObject go in gameObject.scene.GetRootGameObjects()) {
            if (go.layer == chunksLayer) {
                Bounds chunkBounds = go.GetComponent<MeshCollider>().bounds;
                bounds.Encapsulate(chunkBounds.min);
                bounds.Encapsulate(chunkBounds.max);
            }
        }
        return bounds.center;
    }
}
