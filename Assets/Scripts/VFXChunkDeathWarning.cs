using Assets.Code;
using Assets.Code.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXChunkDeathWarning : MonoBehaviour
{
    public MeshRenderer meshRenderer;

    float initialAlpha, vAlpha;

    private void Start() {
        meshRenderer.material = new Material(meshRenderer.material);
        initialAlpha = meshRenderer.material.color.a;
        Color c = meshRenderer.material.color;
        c.a = 0;
        meshRenderer.material.color = c;
    }

    void Update() {
        Chunk chunk = GameStateManagerScript.instance.gameState.chunks[0];
        transform.localPosition = chunk.position + new Vector3(0, chunk.flipZ ? chunk.yOffsetBack : chunk.yOffsetFront, 10);
        Color c = meshRenderer.material.color;
        c.a = Mathf.SmoothDamp(c.a, GameStateManagerScript.instance.gameState.chunkTicks <= Constants.UI_WARN_TICKS ? initialAlpha : 0, ref vAlpha, .5f);
        meshRenderer.material.color = c;
    }
}
