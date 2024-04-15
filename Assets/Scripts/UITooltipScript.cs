using Assets.Code;
using Assets.Code.Model;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UITooltipScript : MonoBehaviour
{
    public static UITooltipScript instance;

    public LayerMask layerMaskWorldTooltips;
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI tmpMain, tmpUpperRight;

    public ITooltippableObject lastHovered;
    public TooltipBehavior hoveredBehavior;
    int layerTooltippable;
    float vAlpha;

    void Start() {
        instance = this;
        layerTooltippable = LayerMask.NameToLayer("UI Tooltippable");
        canvasGroup.alpha = 0;
    } 

    void Update() {
        hoveredBehavior = GetHoveredBehavior();
        ITooltippableObject hovered = hoveredBehavior?.GetTooltippableObject();
        if (hovered != lastHovered) {
            lastHovered = hovered;
            Set();
        }
        canvasGroup.alpha = Mathf.SmoothDamp(canvasGroup.alpha, hovered == null ? 0 : 1, ref vAlpha, .1f);
    }
    void Set() {
        if (lastHovered == null) {
            return;
        }
        Tooltip tooltip = lastHovered.GetTooltip();
        tmpMain.text = $"<line-indent=-2em><line-height=50%><b><size=150%>{tooltip.header}</size></b>\n</line-height>\n{tooltip.content}";
        tmpUpperRight.text = tooltip.upperRight;
    }

    TooltipBehavior GetHoveredBehavior() {
        // UI raycast.
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);
        foreach (RaycastResult result in raycastResults) {
            if (result.gameObject.layer == layerTooltippable) {
                return result.gameObject.GetComponent<TooltipBehavior>();
            }
        }
        // World raycast.
        Collider c = Util.GetMouseCollider(layerMaskWorldTooltips);
        return c?.GetComponent<TooltipBehavior>();
    }
}
