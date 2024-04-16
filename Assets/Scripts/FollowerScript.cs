using Assets.Code;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FollowerScript : MonoBehaviour
{
    public TextMeshPro[] tmpLevelIndicators;

    Vector3 vCenter;

    void Update() {
        // Move.
        Vector3 chunksCenter = Util.GetChunksCenter(gameObject.scene);
        if (transform.localPosition == Vector3.zero) {
            transform.localPosition = chunksCenter;
        } else {
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, chunksCenter, ref vCenter, 1f);
        }
        // Level indicators.
        string levelString = $"LEVEL {GameStateManagerScript.instance.gameState.level + 1}";
        foreach (TextMeshPro tmp in tmpLevelIndicators) {
            tmp.text = levelString;
        }
    }
}
