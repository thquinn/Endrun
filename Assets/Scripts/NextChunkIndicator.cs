using Assets.Code.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextChunkIndicator : MonoBehaviour
{
    void Update() {
        Chunk chunk = GameStateManagerScript.instance.gameState.chunks[1];
        transform.localPosition = chunk.position + new Vector3(0, chunk.flipZ ? chunk.yOffsetBack : chunk.yOffsetFront, 10);
    }
}
