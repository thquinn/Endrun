using Assets.Code.Animation;
using Assets.Code.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitScript : MonoBehaviour
{
    public GameObject prefabHoveringHPPip;

    public NavMeshAgent navMeshAgent;
    public NavMeshObstacle navMeshObstacle;

    public Transform enemyHPSlider;
    public GameObject turnIndicator, turnActionFull, turnActionEmpty;
    public SpriteRenderer turnMovementRenderer;

    public Unit unit;
    GameStateManagerScript gameStateManager;
    List<SpriteRenderer> enemyHPPipOutlines, enemyHPPipFills;

    public void Init(Unit unit) {
        this.unit = unit;
        gameStateManager = GameStateManagerScript.instance;
        transform.localPosition = unit.position;
        if (unit.playerControlled) {
            enemyHPSlider.parent.gameObject.SetActive(false);
        } else {
            enemyHPPipOutlines = new List<SpriteRenderer>();
            enemyHPPipFills = new List<SpriteRenderer>();
        }
    }
    void Start() {
        navMeshAgent.Warp(transform.localPosition);
    }

    void Update() {
        transform.localPosition = unit.position;
        bool showTurnIndicator = unit.playerControlled && gameStateManager.GetActiveUnit() == unit;
        turnIndicator.SetActive(showTurnIndicator);
        if (showTurnIndicator) {
            MoveAnimation moveAnimation = gameStateManager.animationManager.GetCurrentOfType<MoveAnimation>();
            float movementX = moveAnimation?.IsUnitAnimating(unit) == true ? moveAnimation.GetAnimatedMovementRemaining() : unit.movement.x;
            turnMovementRenderer.material.SetFloat("_Revealed", movementX / unit.movement.y);
            turnActionFull.SetActive(unit.actions > 0);
            turnActionEmpty.SetActive(unit.actions <= 0);
        }
        SyncEnemyHP();
    }

    public void ToggleCollider(Unit activeUnit) {
        if (unit == activeUnit) {
            navMeshObstacle.enabled = false;
            navMeshAgent.enabled = true;
        } else {
            // order matters to avoid getting an annoying warning :|
            navMeshAgent.enabled = false;
            navMeshObstacle.enabled = true;
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
        for (int i = 0; i < enemyHPPipOutlines.Count; i++) {
            int xIndex = (i % 5) - 2;
            int rowCount = Mathf.CeilToInt(unit.hp.y / 5f);
            float yIndex = (rowCount - 1) / 2f - (i / 5);
            enemyHPPipOutlines[i].transform.localPosition = new Vector3(xIndex * -.15f, yIndex * .29f, enemyHPPipOutlines[i].transform.localPosition.z);
            enemyHPPipFills[i].enabled = unit.hp.x > i;
        }
    }
}
