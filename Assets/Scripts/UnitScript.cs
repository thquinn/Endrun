using Assets.Code.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitScript : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public NavMeshObstacle navMeshObstacle;

    public Unit unit;

    public void Init(Unit unit) {
        this.unit = unit;
        transform.localPosition = unit.position;
    }
    void Start() {
        navMeshAgent.Warp(transform.localPosition);
    }

    void Update() {
        transform.localPosition = unit.position;
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
