using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXSummonerShineScript : MonoBehaviour
{
    static bool ROTATE = false;

    public SpriteRenderer spriteRenderer;
    public float closeMaxAlpha, farMaxAlpha, closeDistance, farDistance, lifespan, smoothTime, velocity;

    float maxAlpha, vAlpha;
    float time;

    void Start() {
        Color c = spriteRenderer.color;
        c.a = 0;
        spriteRenderer.color = c;
        if (ROTATE) {
            transform.GetChild(0).localRotation = Quaternion.Euler(0, 0, 10);
        }
        ROTATE = !ROTATE;
        float distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        float t = Mathf.Clamp01(Mathf.InverseLerp(closeDistance, farDistance, distance));
        maxAlpha = Mathf.Lerp(closeMaxAlpha, farMaxAlpha, t);
    }

    void Update() {
        time += Time.deltaTime;
        Color c = spriteRenderer.color;
        c.a = Mathf.SmoothDamp(c.a, time < lifespan ? maxAlpha : 0, ref vAlpha, smoothTime);
        spriteRenderer.color = c;
        if (time > lifespan && c.a < .01f) {
            Destroy(gameObject);
        }
        float scale = c.a / maxAlpha * velocity * Time.deltaTime;
        transform.localScale += new Vector3(scale, scale, 1);
    }
}
