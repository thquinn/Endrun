using Assets.Code.Model.Skills;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangePreviewScript : MonoBehaviour
{
    public GameStateManagerScript gameStateManagerScript;

    public GameObject previewSphere, previewRing;

    void Start() {
        
    }

    void Update() {
        RangePreviewType previewType = gameStateManagerScript.gameState.skillDecision?.rangePreviewType ?? RangePreviewType.None;
        float radius = previewType == RangePreviewType.None ? 0 : gameStateManagerScript.gameState.skillDecision.baseRadius;
        radius += .25f; // expand slightly to include the actual models of units in range
        previewSphere.SetActive(previewType == RangePreviewType.Sphere);
        previewRing.SetActive(previewType == RangePreviewType.Ring);
        if (previewType != RangePreviewType.None) {
            transform.localPosition = gameStateManagerScript.gameState.skillDecision.skill.unit.position;
        }
        if (previewType == RangePreviewType.Sphere) {
            previewSphere.transform.localScale = new Vector3(radius, radius, radius) * 2;
        }
        if (previewType == RangePreviewType.Ring) {
            previewRing.transform.localScale = new Vector3(radius, radius, radius) * 2;
        }
    }
}
