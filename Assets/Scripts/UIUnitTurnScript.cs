using Assets.Code;
using Assets.Code.Model;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class UIUnitTurnScript : TooltipBehavior
{
    public static float SPACING = 62;
    static float FADE_TIME = .5f;
    static Vector2 GRAVITY = new Vector2(0, -5f);

    public SpriteAtlas atlasUnitIcons;

    public CanvasGroup canvasGroup;
    public Image imageIcon;
    public TextMeshProUGUI tmpTicksLeft;
    public GameObject outlineGlow;

    public Unit unit;
    GameStateManagerScript gameStateManager;
    RectTransform rt;
    Vector2 v;
    int lastTicksUntilTurn;
    bool skipAnim, fadingOut;

    public void Init(Unit unit, bool skipAnim) {
        this.unit = unit;
        imageIcon.sprite = atlasUnitIcons.GetSprite(unit.iconID);
        Color c = Util.GetUnitUIColor(unit);
        c.a = imageIcon.color.a;
        imageIcon.color = c;
        this.skipAnim = skipAnim;
        gameStateManager = GameStateManagerScript.instance;
        rt = transform as RectTransform;
        UpdateInfo();
    }
    void Start() {
        fadingOut = false;
        v = Vector2.zero;
        float x = GetX();
        Vector2 anchoredPosition = new Vector2(x, 0);
        if (!skipAnim) {
            canvasGroup.alpha = 0;
            if (IsLast()) {
                anchoredPosition.x -= SPACING;
            }
            else {
                anchoredPosition.y -= 100;
            }
        }
        rt.anchoredPosition = anchoredPosition;
        lastTicksUntilTurn = unit.ticksUntilTurn;
    }

    void Update() {
        if (unit.ticksUntilTurn > lastTicksUntilTurn) {
            fadingOut = true;
            v = Vector2.zero;
        }
        lastTicksUntilTurn = unit.ticksUntilTurn;
        if (fadingOut) {
            v += GRAVITY * Time.deltaTime;
            rt.anchoredPosition += v * Time.deltaTime;
            canvasGroup.alpha -= Time.deltaTime / FADE_TIME;
            if (canvasGroup.alpha <= 0) {
                Start();
            }
            return;
        }
        canvasGroup.alpha += Time.deltaTime / FADE_TIME;
        Vector2 targetedPosition = new Vector2(GetX(), 0);
        rt.anchoredPosition = Vector2.SmoothDamp(rt.anchoredPosition, targetedPosition, ref v, .25f);
        outlineGlow.SetActive(UITooltipScript.instance.lastHovered == unit && UITooltipScript.instance.hoveredBehavior != this);
        UpdateInfo();
    }
    void UpdateInfo() {
        tmpTicksLeft.text = unit.ticksUntilTurn == 0 ? "" : $"-{unit.ticksUntilTurn}";
    }

    float GetX() {
        int index = gameStateManager.gameState.units.IndexOf(unit);
        return index * -SPACING;
    }
    bool IsLast() {
        return gameStateManager.gameState.units[gameStateManager.gameState.units.Count - 1] == unit;
    }

    public override ITooltippableObject GetTooltippableObject() {
        return unit;
    }
}
