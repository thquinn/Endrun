using Assets.Code.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitScript : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;

    public Unit unit;

    public void Init(Unit unit) {
        this.unit = unit;
        transform.localPosition = unit.position;
    }
    void Start() {
        navMeshAgent.Warp(transform.localPosition);
    }

    void Update()
    {
        transform.localPosition = unit.position;
    }
}
