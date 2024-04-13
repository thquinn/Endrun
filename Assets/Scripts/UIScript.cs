using Assets.Code.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIScript : MonoBehaviour
{
    public GameObject prefabUILeftUnit, prefabUIUnitTurn;

    public GameStateManagerScript gameStateManagerScript;
    public RectTransform rtLeftUnits, rtTurnOrderList;

    Dictionary<Unit, UILeftUnitScript> leftUnitScripts;
    Dictionary<Unit, UIUnitTurnScript> unitTurnScripts;

    void Start() {
        leftUnitScripts = new Dictionary<Unit, UILeftUnitScript>();
        unitTurnScripts = new Dictionary<Unit, UIUnitTurnScript>();
    }

    void Update() {
        SyncLeftUnits();
        SyncTurnOrder();
    }
    void SyncLeftUnits() {
        List<Unit> units = gameStateManagerScript.gameState.units;
        foreach (Unit unit in units) {
            if (unit.playerControlled && !leftUnitScripts.ContainsKey(unit)) {
                UILeftUnitScript leftUnitScript = Instantiate(prefabUILeftUnit, rtLeftUnits).GetComponent<UILeftUnitScript>();
                leftUnitScript.Init(unit);
                leftUnitScripts[unit] = leftUnitScript;
            }
        }
        var nulls = leftUnitScripts.Keys.Where(u => !units.Contains(u));
        foreach (Unit unit in nulls) {
            Destroy(leftUnitScripts[unit].gameObject);
            leftUnitScripts.Remove(unit);
        }
    }
    void SyncTurnOrder() {
        List<Unit> units = gameStateManagerScript.gameState.units;
        foreach (Unit unit in units) {
            if (!unitTurnScripts.ContainsKey(unit)) {
                UIUnitTurnScript uiUnitTurnScript = Instantiate(prefabUIUnitTurn, rtTurnOrderList).GetComponent<UIUnitTurnScript>();
                uiUnitTurnScript.Init(unit);
                unitTurnScripts[unit] = uiUnitTurnScript;
            }
        }
        var nulls = unitTurnScripts.Keys.Where(u => !units.Contains(u));
        foreach (Unit unit in nulls) {
            Destroy(unitTurnScripts[unit].gameObject);
            unitTurnScripts.Remove(unit);
        }
    }
}
