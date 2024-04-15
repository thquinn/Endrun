using Assets.Code.Model;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageTextScript : MonoBehaviour
{
    static Vector3 GRAVITY = new Vector3(0, -8, 0);

    public TextMeshPro tmp;

    Vector3 v;

    public void Init(Unit unit, int amount) {
        transform.position = unit.position + Vector3.up;
        tmp.text = amount.ToString();
        Color c = tmp.color;
        c.a = 0;
        tmp.color = c;
    }
    void Start() {
        float angle = Random.Range(.45f, .5f) * Mathf.PI;
        v = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
        v = Quaternion.AngleAxis(Random.Range(0, 360f), Vector3.up) * v;
        float magnitude = Random.Range(4f, 5f);
        v *= magnitude;
        // Shove towards camera.
        v += (Camera.main.transform.position - transform.position).normalized;
    }

    void Update() {
        v += GRAVITY * Time.deltaTime;
        transform.localPosition += v * Time.deltaTime;
        bool fading = v.y < 0;
        Color c = tmp.color;
        c.a += (fading ? -1 : 1) * Time.deltaTime * 5;
        if (c.a <= 0) {
            Destroy(gameObject);
            return;
        }
        tmp.color = c;
    }
}
