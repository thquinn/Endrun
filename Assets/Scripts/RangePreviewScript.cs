using Assets.Code;
using Assets.Code.Model.Skills;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangePreviewScript : MonoBehaviour
{
    public GameStateManagerScript gameStateManagerScript;

    public GameObject previewSphere, previewRing, previewCone;
    public LayerMask layerMaskChunks;

    void Start() {
        
    }

    void Update() {
        SkillDecision decision = gameStateManagerScript.gameState.skillDecision;
        RangePreviewType previewType = decision?.rangePreviewType ?? RangePreviewType.None;
        float radius = previewType == RangePreviewType.None ? 0 : decision.baseRadius;
        radius += .25f; // expand slightly to include the actual models of units in range
        previewSphere.SetActive(previewType == RangePreviewType.Sphere);
        previewRing.SetActive(previewType == RangePreviewType.Ring);
        previewCone.SetActive(previewType == RangePreviewType.Cone);
        if (previewType == RangePreviewType.Sphere) {
            previewSphere.transform.localPosition = decision.skill.unit.position;
            previewSphere.transform.localScale = new Vector3(radius, radius, radius) * 2;
        }
        if (previewType == RangePreviewType.Ring) {
            previewRing.transform.localPosition = decision.skill.unit.position;
            previewRing.transform.localScale = new Vector3(radius, radius, radius) * 2;
        }
        if (previewType == RangePreviewType.Cone) {
            previewCone.transform.localPosition = decision.skill.unit.position;
            previewCone.transform.localScale = new Vector3(radius * 2, 1 / Constants.COMBAT_CONE_RANGE_MULTIPLIER, radius * 2);
        }
        // Ground targeting.
        RangePreviewType groundType = decision?.groundTargetingType ?? RangePreviewType.None;
        float groundRadius = decision?.groundTargetingRadius ?? 0;
        Vector3 target = Util.GetMouseCollisionPoint(layerMaskChunks);
        if (target == Vector3.zero) target = new Vector3(-999, -999, -999);
        bool predicateMatch = decision?.predicate?.Invoke(target) ?? false;
        if (groundType == RangePreviewType.Sphere) {
            previewSphere.SetActive(predicateMatch);
            previewSphere.transform.localPosition = target;
            previewSphere.transform.localScale = new Vector3(groundRadius, groundRadius, groundRadius) * 2;
        }
        if (groundType == RangePreviewType.Ring) {
            previewRing.SetActive(predicateMatch);
            previewRing.transform.localPosition = target;
            previewRing.transform.localScale = new Vector3(groundRadius, groundRadius, groundRadius) * 2;
        }
        if (groundType == RangePreviewType.Cone) {
            previewCone.SetActive(predicateMatch);
            previewCone.transform.localPosition = target;
            previewCone.transform.localScale = new Vector3(groundRadius * 2, 1 / Constants.COMBAT_CONE_RANGE_MULTIPLIER, groundRadius * 2);
        }
        // Ground click.
        if (predicateMatch && Input.GetMouseButtonDown(0)) {
            decision.MakeDecision(target);
            MoveUIScript.disableMouseOneFrame = true;
        }
    }
}
