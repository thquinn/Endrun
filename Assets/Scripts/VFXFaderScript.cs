using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VFXFaderScript : MonoBehaviour
{
    public static VFXFaderScript instance;

    public Image image;

    float time;
    bool fadeOut;
    float vAlpha;

    void Start() {
        instance = this;
        if (!Application.isEditor) {
            image.color = Color.black;
        }
    }
    public void FadeOut() {
        fadeOut = true;
    }

    void Update() {
        time += Time.deltaTime;
        if (time < 1) {
            return;
        }
        Color c = image.color;
        c.a = Mathf.SmoothDamp(c.a, fadeOut ? 1 : 0, ref vAlpha, .25f);
        if (c.a > .99f && fadeOut) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        image.color = c;
    }
}
