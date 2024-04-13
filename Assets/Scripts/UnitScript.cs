using Assets.Code.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitScript : MonoBehaviour
{
    Unit unit;

    public void Init(Unit unit) {
        this.unit = unit;
        transform.localPosition = unit.position;
    }
    void Start() {
        
    }

    void Update()
    {
        
    }
}
