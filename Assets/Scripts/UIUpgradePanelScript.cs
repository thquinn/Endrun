using Assets.Code;
using Assets.Code.Model;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIUpgradePanelScript : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI[] tmps;

    PlayerUpgrade[] lastChoice;
    float vAlpha;

    void Update() {
        PlayerUpgrade[] choice = GameStateManagerScript.instance.gameState.playerUpgradeDecision;
        if (choice != lastChoice) {
            lastChoice = choice;
            Set(choice);
        }
        canvasGroup.alpha = Mathf.SmoothDamp(canvasGroup.alpha, lastChoice == null ? 0 : 1, ref vAlpha, .2f);
    }

    void Set(PlayerUpgrade[] choice) {
        if (choice == null) {
            return;
        }
        for (int i = 0; i < choice.Length; i++) {
            tmps[i].text = choice[i].ToString();
        }
    }

    public void ClickLeft(BaseEventData e) {
        if ((e as PointerEventData).button != PointerEventData.InputButton.Left) return;
        ClickIndex(0);
    }
    public void ClickRight(BaseEventData e) {
        if ((e as PointerEventData).button != PointerEventData.InputButton.Left) return;
        ClickIndex(1);
    }
    public void ClickSkip(BaseEventData e) {
        if ((e as PointerEventData).button != PointerEventData.InputButton.Left) return;
        GameStateManagerScript.instance.gameState.playerUpgradeDecision = null;
    }
    void ClickIndex(int i) {
        GameState gameState = GameStateManagerScript.instance.gameState;
        if (gameState.playerUpgradeDecision == null) return;
        lastChoice[i].Apply(gameState);
        GameStateManagerScript.instance.gameState.playerUpgradeDecision = null;
    }
}
