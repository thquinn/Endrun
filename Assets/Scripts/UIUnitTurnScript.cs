using Assets.Code.Model;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIUnitTurnScript : MonoBehaviour
{
    static float FADE_TIME = .5f;
    static Vector2 GRAVITY = new Vector2(0, -5f);

    public CanvasGroup canvasGroup;
    public TextMeshProUGUI tmpTicksLeft;

    public Unit unit;
    GameStateManagerScript gameStateManager;
    RectTransform rt;
    Vector2 v;
    int lastTicksUntilTurn;
    bool skipAnim, fadingOut;

    public void Init(Unit unit, bool skipAnim) {
        this.unit = unit;
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
                anchoredPosition.x -= 72;
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
        UpdateInfo();
    }
    void UpdateInfo() {
        tmpTicksLeft.text = unit.ticksUntilTurn == 0 ? "" : $"-{unit.ticksUntilTurn}";
    }

    float GetX() {
        int index = gameStateManager.gameState.units.IndexOf(unit);
        return index * -62;
    }
    bool IsLast() {
        return gameStateManager.gameState.units[gameStateManager.gameState.units.Count - 1] == unit;
    }
}
