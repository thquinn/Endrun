using Assets.Code.Model.GameEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXScript : MonoBehaviour
{
    public GameObject prefabDamageText;

    public GameStateManagerScript gameStateManagerScript;

    void Start() {
        gameStateManagerScript.Listen(
            GameEventType.Damage,
            null,
            OnDamage
        );
    }

    bool OnDamage(GameEvent e) {
        DamageTextScript script = Instantiate(prefabDamageText).GetComponent<DamageTextScript>();
        script.Init(e.unitTarget, Mathf.RoundToInt(e.amount));
        return false;
    }
}
