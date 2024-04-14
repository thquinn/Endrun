using Assets.Code.Model.GameEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXScript : MonoBehaviour
{
    public static VFXScript instance;

    public GameObject prefabDamageText;

    public GameStateManagerScript gameStateManagerScript;

    void Start() {
        instance = this;
    }

    bool OnDamage(GameEvent e) {
        DamageTextScript script = Instantiate(prefabDamageText).GetComponent<DamageTextScript>();
        script.Init(e.unitTarget, Mathf.RoundToInt(e.amount));
        return false;
    }

    // "Dumb-style" events.
    public void HandleDamage(GameEvent e) {
        OnDamage(e);
    }
}
