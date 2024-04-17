using Assets.Code;
using Assets.Code.Animation;
using Assets.Code.Model;
using Assets.Code.Model.GameEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Force.DeepCloner;
using Unity.AI.Navigation;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class GameStateManagerScript : MonoBehaviour
{
    public static GameStateManagerScript instance;

    public static GameEventType[] UNDO_HISTORY_EVENT_TYPES = new GameEventType[] { GameEventType.BeforeMove, GameEventType.BeforeResolveSkill };

    public GameObject[] prefabChunks;
    public GameObject prefabUnit, prefabManaCrystal;
    public LayerMask layerMaskUnits;

    public NavMeshSurface navMeshSurface;

    public GameState gameState;
    public Stack<GameState> undoHistory;
    public Dictionary<Chunk, ChunkInfoScript> chunkScripts;
    public Dictionary<Unit, UnitScript> unitScripts;
    public Dictionary<Collider, Unit> unitColliders;
    public Dictionary<ManaCrystal, GameObject> manaCrystalObjects;
    public AnimationManager animationManager;
    public Unit hoveredUnit;

    float[] chunkWeights;

    void Start() {
        instance = this;
        chunkWeights = prefabChunks.Select(p => p.GetComponent<ChunkInfoScript>().weight).ToArray();
        gameState = new GameState();
        undoHistory = new Stack<GameState>();
        chunkScripts = new Dictionary<Chunk, ChunkInfoScript>();
        unitScripts = new Dictionary<Unit, UnitScript>();
        unitColliders = new Dictionary<Collider, Unit>();
        manaCrystalObjects = new Dictionary<ManaCrystal, GameObject>();
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
            gameState.chunkTicks += 1000;
        }
        if (Input.GetKeyDown(KeyCode.F2)) {
            gameState.AddChunk();
        }
        if (Input.GetKeyDown(KeyCode.F3)) {
            gameState.InitPlayerDecision();
        }
        // END DEBUG
        SyncChunks();
        SyncUnits();
        SyncMana();
        if (!gameState.IsGameOver()) {
            SkillDecision();
            EnemyActs();
            animationManager.Update();
        }
    }
    void SyncChunks(bool undo = false) {
        bool changes = false;
        for (int i = 0; i < gameState.chunks.Count; i++) {
            Chunk chunk = gameState.chunks[i];
            if (!chunkScripts.ContainsKey(chunk)) {
                ChunkInfoScript chunkScript = Instantiate(prefabChunks[chunk.index]).GetComponent<ChunkInfoScript>();
                chunkScript.transform.localScale = new Vector3(chunkScript.transform.localScale.x * (chunk.flipX ? -1 : 1), 1, chunkScript.transform.localScale.z * (chunk.flipZ ? -1 : 1));
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
                chunk.yOffsetBack = chunkScript.yOffsetBack;
                chunk.yOffsetFront = chunkScript.yOffsetFront;
            }
        }
        var nulls = chunkScripts.Keys.Where(c => !gameState.chunks.Contains(c)).ToArray();
        foreach (Chunk chunk in nulls) {
            chunkScripts[chunk].Destroy();
            chunkScripts.Remove(chunk);
            changes = true;
        }
        if (changes) {
            foreach (UnitScript unitScript in unitScripts.Values) {
                unitScript.ToggleCollider(false);
            }
            navMeshSurface.BuildNavMesh();
            if (!undo) {
                KillOffMeshUnits();
                undoHistory.Clear();
                Balance.InitializeNewChunk(gameState);
                if (!gameState.IsGameOver() && gameState.level > 0) {
                    gameState.InitPlayerDecision();
                }
            }
            ToggleColliders();
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
                NavMeshAgent agent = unitScript.GetComponent<NavMeshAgent>();
                agent.updatePosition = false;
                agent.updateRotation = false;
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
            navMeshSurface.BuildNavMesh();
        }
    }
    void SyncMana() {
        foreach (ManaCrystal manaCrystal in gameState.manaCrystals) {
            if (!manaCrystalObjects.ContainsKey(manaCrystal)) {
                manaCrystalObjects[manaCrystal] = Instantiate(prefabManaCrystal);
            }
        }
        var nulls = manaCrystalObjects.Keys.Where(m => !gameState.manaCrystals.Contains(m)).ToArray();
        foreach (ManaCrystal manaCrystal in nulls) {
            Destroy(manaCrystalObjects[manaCrystal].gameObject);
            manaCrystalObjects.Remove(manaCrystal);
        }
        foreach (var kvp in manaCrystalObjects) {
            kvp.Value.transform.position = kvp.Key.position;
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
            unitScript.ToggleCollider(unitScript.unit != activeUnit);
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
        gameState.RemoveOldMana();
    }

    public Unit GetActiveUnit() {
        return gameState.GetActiveUnit();
    }
    public bool PlayerCanInput() {
        return !animationManager.IsAnythingAnimating() && gameState.playerUpgradeDecision == null;
    }
    public static bool IsGameOver() {
        return instance.gameState.IsGameOver();
    }


    public int GetRandomChunkIndex() {
        float sum = chunkWeights.Sum();
        float selector = Random.Range(0, sum);
        for (int i = 0; i < chunkWeights.Length; i++) {
            if (selector <= chunkWeights[i]) {
                return i;
            }
            selector -= chunkWeights[i];
        }
        return -1;
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
        SyncChunks(true);
        SyncUnits();
        ToggleColliders();
    }

    // "Dumb-style" events.
    public void HandleUndoCheckpointEvent(GameEvent e) {
        if (GetActiveUnit()?.playerControlled == true) {
            undoHistory.Push(gameState.DeepClone());
        }
    }
    public void HandleTurnStart(GameEvent e) {
        ToggleColliders();
    }
}
