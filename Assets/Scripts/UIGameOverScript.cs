using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIGameOverScript : MonoBehaviour
{
    public CanvasGroup canvasGroup;

    float vAlpha;

    void Update() {
        bool gameOver = GameStateManagerScript.IsGameOver();
        canvasGroup.alpha = Mathf.SmoothDamp(canvasGroup.alpha, !gameOver ? 0 : 1, ref vAlpha, .2f);
        if (gameOver && Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
