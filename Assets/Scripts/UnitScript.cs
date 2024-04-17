using Assets.Code.Animation;
using Assets.Code.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitScript : TooltipBehavior
{
    public GameObject[] prefabUnitMeshes;
    public GameObject prefabHoveringHPPip, prefabSummonerShine;
    public Material materialAlly, materialEnemy;

    public NavMeshAgent navMeshAgent;
    public NavMeshObstacle navMeshObstacle;
    public AudioSource sfxHit, sfxMove;

    public Transform meshContainer, enemyHPSlider;
    public GameObject turnIndicator, turnActionFull, turnActionEmpty;
    public SpriteRenderer turnMovementRenderer, turnSpinnerRenderer;
    public float summonerShinePeriod;

    public Unit unit;
    GameStateManagerScript gameStateManager;
    List<SpriteRenderer> enemyHPPipOutlines, enemyHPPipFills;
    float summonerShineTimer;
    float vTurnSpinnerAlpha, vSFXMove;
    List<Vector3> lastPositions;
    int lastHP;
    bool destroyed;

    public void Init(Unit unit) {
        this.unit = unit;
        gameStateManager = GameStateManagerScript.instance;
        lastPositions = new List<Vector3>();
        transform.localPosition = unit.position;
        lastHP = unit.hp.x;
        // Mesh.
        if (meshContainer.childCount > 0) {
            Destroy(meshContainer.GetChild(0).gameObject);
        }
        foreach (GameObject mesh in prefabUnitMeshes) {
            if (mesh.name == unit.iconID) {
                Transform meshes = Instantiate(mesh, meshContainer).transform;
                if (!unit.isSummoner) {
                    foreach (MeshRenderer meshRenderer in meshes.GetComponentsInChildren<MeshRenderer>()) {
                        meshRenderer.material = unit.playerControlled ? materialAlly : materialEnemy;
                    }
                }
            }
        }
        // HP.
        if (unit.playerControlled) {
            enemyHPSlider.parent.gameObject.SetActive(false);
        } else {
            enemyHPPipOutlines = new List<SpriteRenderer>();
            enemyHPPipFills = new List<SpriteRenderer>();
        }
    }
    void Start() {
        navMeshAgent.Warp(transform.localPosition);
        Color c = turnSpinnerRenderer.color;
        c.a = 0;
        turnSpinnerRenderer.color = c;
        Update();
    }
    public void Destroy() {
        foreach (Transform child in transform) {
            child.gameObject.SetActive(child.tag == "SFX");
        }
        destroyed = true;
    }

    void Update() {
        if (destroyed ) {
            if (!sfxHit.isPlaying) {
                Destroy(gameObject);
            }
            return;
        }
        meshContainer.gameObject.SetActive(!unit.isSummoner || !GameStateManagerScript.IsGameOver());
        transform.localPosition = unit.position;
        bool isActive = gameStateManager.GetActiveUnit() == unit;
        bool showTurnIndicator = isActive && unit.playerControlled;
        turnIndicator.SetActive(showTurnIndicator);
        if (showTurnIndicator) {
            MoveAnimation moveAnimation = gameStateManager.animationManager.GetCurrentOfType<MoveAnimation>();
            float movementX = moveAnimation?.IsUnitAnimating(unit) == true ? moveAnimation.GetAnimatedMovementRemaining() : unit.movement.x;
            turnMovementRenderer.material.SetFloat("_Revealed", movementX / unit.movement.y);
            turnActionFull.SetActive(unit.actions > 0);
            turnActionEmpty.SetActive(unit.actions <= 0);
        }
        // Enemy stuff.
        bool showSpinner = isActive && !unit.playerControlled && !gameStateManager.animationManager.IsAnythingAnimating() && !unit.gameState.IsGameOver();
        Color c = turnSpinnerRenderer.color;
        c.a = Mathf.SmoothDamp(c.a, showSpinner ? 1 : 0, ref vTurnSpinnerAlpha, .1f);
        turnSpinnerRenderer.color = c;
        turnSpinnerRenderer.transform.Rotate(0, 0, Time.deltaTime * -200);
        SyncEnemyHP();
        // VFX.
        if (unit.isSummoner) {
            summonerShineTimer += Time.deltaTime;
            if (summonerShineTimer > summonerShinePeriod) {
                Instantiate(prefabSummonerShine, meshContainer.transform);
                summonerShineTimer -= summonerShinePeriod;
            }
        }
        // SFX.
        lastPositions.Add(unit.position);
        if (lastPositions.Count >= 3) {
            float minDistance = Mathf.Min((lastPositions[0] - lastPositions[1]).magnitude, (lastPositions[1] - lastPositions[2]).magnitude); // check two delta to not play loud sound on teleport
            float targetMoveVolume = minDistance / Time.deltaTime * .04f;
            targetMoveVolume = Mathf.Min(targetMoveVolume, .5f);
            sfxMove.volume = Mathf.SmoothDamp(sfxMove.volume, targetMoveVolume, ref vSFXMove, .05f);
            lastPositions.RemoveAt(0);
        }
        if (unit.hp.x < lastHP) {
            sfxHit.Play();
            lastHP = unit.hp.x;
        }
    }

    public void ToggleCollider(bool on) {
        if (on) {
            // order matters to avoid getting an annoying warning :|
            navMeshAgent.enabled = false;
            navMeshObstacle.enabled = true;
        } else {
            navMeshObstacle.enabled = false;
            navMeshAgent.enabled = true;
        }
    }

    void SyncEnemyHP() {
        if (!enemyHPSlider.gameObject.activeInHierarchy) {
            return;
        }
        int pipDelta = unit.hp.y - enemyHPPipOutlines.Count;
        for (; pipDelta > 0; pipDelta--) {
            SpriteRenderer outlineRenderer = Instantiate(prefabHoveringHPPip, enemyHPSlider).GetComponent<SpriteRenderer>();
            enemyHPPipOutlines.Add(outlineRenderer);
            enemyHPPipFills.Add(outlineRenderer.transform.GetChild(0).GetComponent<SpriteRenderer>());
        }
        for (int i = 0; i > pipDelta; i--) {
            Destroy(enemyHPPipOutlines[enemyHPPipOutlines.Count - 1].gameObject);
            enemyHPPipOutlines.RemoveAt(enemyHPPipOutlines.Count - 1);
            enemyHPPipFills.RemoveAt(enemyHPPipFills.Count - 1);
        }
        int rowSize = enemyHPPipOutlines.Count <= 10 ? 5 : 10;
        enemyHPSlider.transform.localScale = enemyHPPipOutlines.Count <= 10 ? Vector3.one : new Vector3(.5f, .5f, 1);
        for (int i = 0; i < enemyHPPipOutlines.Count; i++) {
            int xIndex = (i % rowSize) - (rowSize / 2);
            int rowCount = Mathf.CeilToInt(unit.hp.y / (float)rowSize);
            float yIndex = (rowCount - 1) / 2f - (i / rowSize);
            enemyHPPipOutlines[i].transform.localPosition = new Vector3(xIndex * -.15f, yIndex * .29f, enemyHPPipOutlines[i].transform.localPosition.z);
            enemyHPPipFills[i].enabled = unit.hp.x > i;
        }
    }

    public override ITooltippableObject GetTooltippableObject() {
        return unit;
    }
}
