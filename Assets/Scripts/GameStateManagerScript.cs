using Assets.Code;
using Assets.Code.Animation;
using Assets.Code.Model;
using Assets.Code.Model.GameEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Force.DeepCloner;
using UnityEditor.AI;

public class GameStateManagerScript : MonoBehaviour
{
    public static GameStateManagerScript instance;

    public static GameEventType[] UNDO_HISTORY_EVENT_TYPES = new GameEventType[] { GameEventType.BeforeMove, GameEventType.BeforeResolveSkill };

    public GameObject[] prefabChunks;
    public GameObject prefabUnit;
    public LayerMask layerMaskUnits;

    public GameState gameState;
    public Stack<GameState> undoHistory;
    public Dictionary<Chunk, GameObject> chunkObjects;
    public Dictionary<Unit, UnitScript> unitScripts;
    public Dictionary<Collider, Unit> unitColliders;
    public AnimationManager animationManager;
    public Unit hoveredUnit;

    void Start() {
        instance = this;
        gameState = new GameState();
        undoHistory = new Stack<GameState>();
        chunkObjects = new Dictionary<Chunk, GameObject>();
        unitScripts = new Dictionary<Unit, UnitScript>();
        unitColliders = new Dictionary<Collider, Unit>();
        SyncChunks();
        SyncUnits();
        animationManager = new AnimationManager();
        gameState.gameEventManager.Trigger(new GameEvent() {
            type = GameEventType.TurnStart,
        });
        gameState.units[0].StartTurn();
    }

    void Update() {
        SyncChunks();
        SyncUnits();
        SkillDecision();
        EnemyActs();
        animationManager.Update();
    }
    void SyncChunks() {
        return;
        bool changes = false;
        foreach (Chunk chunk in gameState.chunks) {
            if (!chunkObjects.ContainsKey(chunk)) {
                GameObject chunkObject = Instantiate(prefabChunks[chunk.index]);
                chunkObjects[chunk] = chunkObject;
                changes = true;
            }
        }
        var nulls = chunkObjects.Keys.Where(c => !gameState.chunks.Contains(c)).ToArray();
        foreach (Chunk chunk in nulls) {
            Destroy(chunkObjects[chunk]);
            chunkObjects.Remove(chunk);
            changes = true;
        }
        if (changes) {
            NavMeshBuilder.BuildNavMesh();
        }
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
    void EnemyActs() {
        Unit activeUnit = GetActiveUnit();
        if (!activeUnit.playerControlled && !animationManager.IsAnythingAnimating()) {
            EnemyAI.DecideWithDelay(activeUnit);
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

    bool ToggleColliders() {
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

    public void Undo() {
        if (undoHistory.Count == 0) return;
        gameState = undoHistory.Pop();
        SyncUnits();
        ToggleColliders();
    }

    // "Dumb-style" events.
    public void HandleUndoCheckpointEvent(GameEvent e) {
        if (GetActiveUnit().playerControlled) {
            undoHistory.Push(gameState.DeepClone());
        }
    }
    public void HandleTurnStart(GameEvent e) {
        ToggleColliders();
    }
}
