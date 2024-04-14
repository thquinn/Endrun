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
using Unity.AI.Navigation;
using UnityEngine.AI;

public class GameStateManagerScript : MonoBehaviour
{
    public static GameStateManagerScript instance;

    public static GameEventType[] UNDO_HISTORY_EVENT_TYPES = new GameEventType[] { GameEventType.BeforeMove, GameEventType.BeforeResolveSkill };

    public GameObject[] prefabChunks;
    public GameObject prefabUnit;
    public LayerMask layerMaskUnits;

    public NavMeshSurface navMeshSurface;

    public GameState gameState;
    public Stack<GameState> undoHistory;
    public Dictionary<Chunk, ChunkInfoScript> chunkScripts;
    public Dictionary<Unit, UnitScript> unitScripts;
    public Dictionary<Collider, Unit> unitColliders;
    public AnimationManager animationManager;
    public Unit hoveredUnit;

    void Start() {
        instance = this;
        gameState = new GameState();
        undoHistory = new Stack<GameState>();
        chunkScripts = new Dictionary<Chunk, ChunkInfoScript>();
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
        // DEBUG DEBUG DEBUG
        if (Input.GetKeyDown(KeyCode.F1)) {
            gameState.chunks.Add(new Chunk(0, UnityEngine.Random.value < .5f, UnityEngine.Random.value < .5f));
        }
        // END DEBUG
        SyncChunks();
        SyncUnits();
        SkillDecision();
        EnemyActs();
        animationManager.Update();
    }
    void SyncChunks(bool kill = true) {
        bool changes = false;
        for (int i = 0; i < gameState.chunks.Count; i++) {
            Chunk chunk = gameState.chunks[i];
            if (!chunkScripts.ContainsKey(chunk)) {
                ChunkInfoScript chunkScript = Instantiate(prefabChunks[chunk.index]).GetComponent<ChunkInfoScript>();
                chunkScript.transform.localScale = new Vector3(chunk.flipX ? -1 : 1, 1, chunk.flipZ ? -1 : 1);
                chunkScripts[chunk] = chunkScript;
                changes = true;
                if (i > 0) {
                    // Set the new chunk's position properly.
                    Chunk previousChunk = gameState.chunks[i - 1];
                    ChunkInfoScript previousInfo = chunkScripts[previousChunk];
                    float previousYOffset = previousChunk.flipZ ? previousInfo.yOffsetBack : previousInfo.yOffsetFront;
                    float thisYOffset = chunk.flipZ ? chunkScript.yOffsetFront : chunkScript.yOffsetBack;
                    chunk.position = previousChunk.position;
                    chunk.position.y += previousYOffset - thisYOffset;
                    chunk.position.z += 20;
                    chunkScript.transform.position = chunk.position;
                }
            }
        }
        var nulls = chunkScripts.Keys.Where(c => !gameState.chunks.Contains(c)).ToArray();
        foreach (Chunk chunk in nulls) {
            chunkScripts[chunk].gameObject.SetActive(false);
            Destroy(chunkScripts[chunk].gameObject);
            chunkScripts.Remove(chunk);
            changes = true;
        }
        if (changes) {
            navMeshSurface.BuildNavMesh();
            if (kill) {
                KillOffMeshUnits();
            }
        }
    }
    void SyncUnits() {
        bool changes = false;
        foreach (Unit unit in gameState.units) {
            if (!unitScripts.ContainsKey(unit)) {
                UnitScript unitScript = Instantiate(prefabUnit).GetComponent<UnitScript>();
                unitScript.Init(unit);
                unitScripts[unit] = unitScript;
                unitColliders[unitScript.GetComponent<Collider>()] = unit;
                changes = true;
            }
        }
        var nulls = unitScripts.Keys.Where(u => !gameState.units.Contains(u)).ToArray();
        foreach (Unit unit in nulls) {
            Destroy(unitScripts[unit].gameObject);
            unitScripts.Remove(unit);
            changes = true;
        }
        if (changes) {
            ToggleColliders();
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
        if (Input.GetMouseButtonDown(0) && gameState.skillDecision?.IsValid(hoveredUnit) == true) {
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
    void KillOffMeshUnits() {
        foreach (Unit unit in gameState.units) {
            NavMeshHit navMeshHit;
            NavMesh.SamplePosition(unit.position, out navMeshHit, 3f, NavMesh.AllAreas);
            if (!navMeshHit.hit || Vector3.Distance(unit.position, navMeshHit.position) > .33f) {
                unit.Die();
            }
        }
        gameState.RemoveDeadUnits();
    }

    public Unit GetActiveUnit() {
        return gameState.units.Count == 0 ? null : gameState.units[0];
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
        SyncChunks(false);
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
