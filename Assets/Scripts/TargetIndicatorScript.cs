using Assets.Code;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetIndicatorScript : MonoBehaviour
{
    static int NUM_ARROWS = 6;

    float spin;
    float pulse;
    float sphincter;

    public UnitScript unitScript;
    List<SpriteRenderer> arrowRenderers, glowRenderers;
    float vAlpha, vGlowAlpha, vSphincter;

    void Start() {
        arrowRenderers = new List<SpriteRenderer>();
        glowRenderers = new List<SpriteRenderer>();
        GameObject arrowPivot = transform.GetChild(0).gameObject;
        for (int i = 1; i < NUM_ARROWS; i++) {
            GameObject arrowCopy = Instantiate(arrowPivot, transform);
            arrowCopy.transform.Rotate(0, 0, 360f / NUM_ARROWS * i);
        }
        foreach (Transform child in transform) {
            Transform arrow = child.GetChild(0);
            SpriteRenderer arrowRenderer = arrow.GetComponent<SpriteRenderer>();
            Color c = arrowRenderer.color;
            c.a = 0;
            arrowRenderer.color = c;
            arrowRenderers.Add(arrowRenderer);
            SpriteRenderer glowRenderer = arrow.GetChild(0).GetComponent<SpriteRenderer>();
            c = glowRenderer.color;
            c.a = 0;
            glowRenderer.color = c;
            glowRenderers.Add(glowRenderer);
        }
    }

    void Update() {
        bool targetable = GameStateManagerScript.instance.gameState.skillDecision?.IsValid(unitScript.unit) == true;
        bool tooltipped = UITooltipScript.instance.lastHovered == unitScript.unit;
        bool active = targetable || tooltipped;
        bool targeting = targetable && GameStateManagerScript.instance.hoveredUnit == unitScript.unit;
        if (!active && arrowRenderers[0].color.a <= 0) return;

        spin += Time.deltaTime * (targeting ? 1.5f : .66f);
        Vector3 spinVector = new Vector3(Mathf.Cos(spin), 0, Mathf.Sin(spin));
        transform.rotation = Quaternion.LookRotation(NavMeshUtil.GetChunksNormal(transform.position), spinVector);

        float targetAlpha = active ? 1 : 0;
        float alpha = Mathf.SmoothDamp(arrowRenderers[0].color.a, targetAlpha, ref vAlpha, .1f);
        foreach (SpriteRenderer arrowRenderer in arrowRenderers) {
            Color c = arrowRenderer.color;
            c.a = alpha;
            arrowRenderer.color = c;
        }

        pulse += Time.deltaTime * 5;
        float targetGlowAlpha = targeting ? Mathf.Cos(pulse) / 2 + .5f : 0;
        float glowAlpha = Mathf.SmoothDamp(glowRenderers[0].color.a, targetGlowAlpha, ref vGlowAlpha, .1f);
        foreach (SpriteRenderer glowRenderer in glowRenderers) {
            Color c = glowRenderer.color;
            c.a = glowAlpha;
            glowRenderer.color = c;
        }

        sphincter += Time.deltaTime * 4;
        float targetY = -.5f;
        if (targeting) {
            targetY += Mathf.Sin(sphincter) * .1f;
        }
        float y = Mathf.SmoothDamp(transform.GetChild(0).GetChild(0).localPosition.y, targetY, ref vSphincter, .2f);
        foreach (Transform child in transform) {
            child.GetChild(0).transform.localPosition = new Vector3(0, y, 0);
        }
    }
}
