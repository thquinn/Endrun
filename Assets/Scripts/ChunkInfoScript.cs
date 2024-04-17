using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkInfoScript : MonoBehaviour {
    static float GRAVITY = -9.8f;

    public Material materialChunkFade;

    public float yOffsetBack, yOffsetFront, weight;

    bool destroying;
    Material fadeMaterialCopy;
    float dy, dAlpha;

    public void Destroy() {
        destroying = true;
        gameObject.layer = 0;
        SetGameLayerRecursive(gameObject, 0);
        fadeMaterialCopy = new Material(materialChunkFade);
        foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>()) {
            mr.material = fadeMaterialCopy;
        }
    }
    private void Update() {
        if (!destroying) return;
        dy += GRAVITY * Time.deltaTime;
        transform.Translate(0, dy * Time.deltaTime, 0);
        Color c = fadeMaterialCopy.color;
        c.a = Mathf.SmoothDamp(c.a, 0, ref dAlpha, 1f);
        if (c.a < .01f) {
            Destroy(gameObject);
        }
        fadeMaterialCopy.color = c;
    }

    // courtesy of ignacio-casal on the Unity forums
    private void SetGameLayerRecursive(GameObject gameObject, int layer) {
        gameObject.layer = layer;
        foreach (Transform child in gameObject.transform) {
            SetGameLayerRecursive(child.gameObject, layer);
        }
    }
}
