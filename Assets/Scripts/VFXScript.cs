using Assets.Code.Model.GameEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXScript : MonoBehaviour, IGameEventMonoBehaviourHandler
{
    public GameObject prefabDamageText;

    public GameStateManagerScript gameStateManagerScript;

    void Start() {
        gameStateManagerScript.Listen(
            GameEventType.Damage,
            null,
            this,
            gameStateManagerScript.gameState.gameEventManager
        );
    }
    public void Reregister(GameEventManager gameEventManager) {
        gameStateManagerScript.Listen(
            GameEventType.Damage,
            null,
            this,
            gameEventManager
        );
    }
    public bool Handle(GameEvent e) {
        DamageTextScript script = Instantiate(prefabDamageText).GetComponent<DamageTextScript>();
        script.Init(e.unitTarget, Mathf.RoundToInt(e.amount));
        return false;
    }
}
