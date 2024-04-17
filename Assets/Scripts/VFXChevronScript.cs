using Assets.Code;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXChevronScript : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public float maxAlpha, lifespan, smoothTime, velocity;

    float vAlpha;
    float time;

    void Start() {
        Color c = spriteRenderer.color;
        c.a = 0;
        spriteRenderer.color = c;
    }

    void Update() {
        float lifespanActual = lifespan * 2;
        time += Time.deltaTime;
        Color c1 = time < lifespanActual / 2 ? Constants.COLOR_ENEMY : Constants.COLOR_ALLY;
        float t = Mathf.Min(time, lifespanActual) / lifespanActual * 2;
        if (t > 1) t = 2 - t;
        t = Mathf.Pow(t, 2);
        Color rgb = Color.Lerp(c1, Color.white, t);
        rgb.a = Mathf.SmoothDamp(spriteRenderer.color.a, time < lifespan ? maxAlpha : 0, ref vAlpha, smoothTime);
        spriteRenderer.color = rgb;
        if (time > lifespan && rgb.a < .01f) {
            Destroy(gameObject);
        }
        transform.Translate(0, rgb.a / maxAlpha * velocity * Time.deltaTime, 0);
    }
}
