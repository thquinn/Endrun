using Assets.Code;
using Assets.Code.Animation;
using Assets.Code.Model;
using Assets.Code.Model.GameEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Force.DeepCloner;

public class GameStateManagerScript : MonoBehaviour
{
    public static GameStateManagerScript instance;

    public GameObject prefabUnit;
    public LayerMask layerMaskUnits;

    public GameState gameState;
    public Dictionary<Unit, UnitScript> unitScripts;
    public Dictionary<Collider, Unit> unitColliders;
    public AnimationManager animationManager;
    public Unit hoveredUnit;

    GameState savedState;

    void Start() {
        instance = this;
        gameState = new GameState();
        unitScripts = new Dictionary<Unit, UnitScript>();
        unitColliders = new Dictionary<Collider, Unit>();
        SyncUnits();
        animationManager = new AnimationManager();
        Listen(
            GameEventType.TurnStart,
            null,
            UpdateNavMesh
        );
        gameState.gameEventManager.Trigger(new GameEvent() {
            type = GameEventType.TurnStart,
        });
        gameState.units[0].StartTurn();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.F5)) {
            savedState = gameState.DeepClone();
        }
        else if (Input.GetKeyDown(KeyCode.F8)) {
            LoadState(savedState);
        }
        SyncUnits();
        SkillDecision();
        animationManager.Update();
    }
    void SyncUnits() {
        foreach (Unit unit in gameState.units) {
            if (!unitScripts.ContainsKey(unit)) {
                UnitScript unitScript = Instantiate(prefabUnit).GetComponent<UnitScript>();
                unitScript.Init(unit);
                unitScripts[unit] = unitScript;
                unitColliders[unitScript.GetComponent<Collider>()] = unit;
            }
        }
        var nulls = unitScripts.Keys.Where(u => !gameState.units.Contains(u)).ToArray();
        foreach (Unit unit in nulls) {
            Destroy(unitScripts[unit].gameObject);
            unitScripts.Remove(unit);
        }
    }
    void SkillDecision() {
        Collider hoveredUnitCollider = Util.GetMouseCollider(layerMaskUnits);
        hoveredUnit = (hoveredUnitCollider != null && unitColliders.ContainsKey(hoveredUnitCollider)) ? unitColliders[hoveredUnitCollider] : null;
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1)) {
            gameState.skillDecision = null;
            return;
        }
        if (Input.GetMouseButtonDown(0) && gameState.skillDecision?.IsAChoice(hoveredUnit) == true) {
            gameState.skillDecision.MakeDecision(hoveredUnit);
            MoveUIScript.disableMouseOneFrame = true;
        }
    }

    bool UpdateNavMesh(GameEvent e) {
        Unit activeUnit = GetActiveUnit();
        foreach (UnitScript unitScript in unitScripts.Values) {
            unitScript.ToggleCollider(activeUnit);
        }
        return false;
    }

    public Unit GetActiveUnit() {
        return gameState.units[0];
    }

    public void Listen(GameEventType type, Predicate<GameEvent> predicate, Func<GameEvent, bool> func) {
        gameState.gameEventManager.Listen(type, predicate, func);
    }
    public void EnqueueAnimation(AnimationBase animation) {
        animationManager.Enqueue(animation);
    }

    // Undo support.
    void LoadState(GameState saved) {
        gameState = saved;
        SyncUnits();
        UpdateNavMesh(null);
    }
}
