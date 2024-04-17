using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIGameOverScript : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI tmp;

    float vAlpha;

    void Update() {
        bool gameOver = GameStateManagerScript.IsGameOver();
        canvasGroup.alpha = Mathf.SmoothDamp(canvasGroup.alpha, !gameOver ? 0 : 1, ref vAlpha, .2f);
        tmp.text = GameStateManagerScript.instance.undoHistory.Count == 0 ?
            "Press <b>R</b> to start again." :
            "<b>Undo</b> to right your wrongs, or press <b>R</b> to start again.";
        if (gameOver && Input.GetKeyDown(KeyCode.R)) {
            VFXFaderScript.instance.FadeOut();
        }
    }
}
