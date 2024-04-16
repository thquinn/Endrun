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
        time += Time.deltaTime;
        Color c = spriteRenderer.color;
        c.a = Mathf.SmoothDamp(c.a, time < lifespan ? maxAlpha : 0, ref vAlpha, smoothTime);
        spriteRenderer.color = c;
        if (time > lifespan && c.a < .01f) {
            Destroy(gameObject);
        }
        transform.Translate(0, c.a / maxAlpha * velocity * Time.deltaTime, 0);
    }
}
