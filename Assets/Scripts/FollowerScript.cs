using Assets.Code;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FollowerScript : MonoBehaviour
{
    static float CHEVRON_PERIOD = 3f;

    public GameObject prefabChevron;

    public TextMeshPro[] tmpLevelIndicators;

    Vector3 vCenter;
    float chevronTimer;

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
        // Chevrons.
        chevronTimer += Time.deltaTime;
        if (chevronTimer >= CHEVRON_PERIOD) {
            Instantiate(prefabChevron).transform.position = transform.position + new Vector3(-15, -2, -15);
            Instantiate(prefabChevron).transform.position = transform.position + new Vector3(15, -2, -15);
            chevronTimer -= CHEVRON_PERIOD;
        }
    }
}
