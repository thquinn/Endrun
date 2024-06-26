using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonHoverAlphaScript : MonoBehaviour
{
    public Image image;
    public float deltaAlpha;

    bool hovered;
    float initialAlpha, vAlpha;

    void Start() {
        initialAlpha = image.color.a;
    }

    void Update() {
        Color c = image.color;
        c.a = Mathf.SmoothDamp(c.a, !hovered ? initialAlpha : initialAlpha + deltaAlpha, ref vAlpha, .1f);
        image.color = c;
    }

    public void OnPointerEnter() {
        hovered = true;   
    }
    public void OnPointerExit() {
        hovered = false;
    }
}
