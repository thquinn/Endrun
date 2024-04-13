using Assets.Code.Animation;
using Assets.Code.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitScript : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public NavMeshObstacle navMeshObstacle;

    public GameObject turnIndicator;
    public SpriteRenderer turnMovementRenderer;

    public Unit unit;
    GameStateManagerScript gameStateManager;

    public void Init(Unit unit) {
        this.unit = unit;
        gameStateManager = GameStateManagerScript.instance;
        transform.localPosition = unit.position;
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
        }
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
}
