using Assets.Code.Animation;
using Assets.Code.Model;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameStateManagerScript : MonoBehaviour
{
    public static GameStateManagerScript instance;

    public GameObject prefabUnit;

    public GameState gameState;
    public Dictionary<Unit, UnitScript> unitScripts;
    public AnimationManager animationManager;

    void Start() {
        instance = this;
        gameState = new GameState();
        unitScripts = new Dictionary<Unit, UnitScript>();
        animationManager = new AnimationManager();
    }

    void Update() {
        foreach (Unit unit in gameState.units) {
            if (!unitScripts.ContainsKey(unit)) {
                UnitScript unitScript = Instantiate(prefabUnit).GetComponent<UnitScript>();
                unitScript.Init(unit);
                unitScripts[unit] = unitScript;
            }
        }
        var nulls = unitScripts.Where(kvp => kvp.Value == null).Select(kvp => kvp.Key);
        foreach (Unit unit in nulls) {
            unitScripts.Remove(unit);
        }
        animationManager.Update();
    }

    public Unit GetSelectedUnit() {
        return gameState.units[0];
    }

    public void EnqueueAnimation(AnimationBase animation) {
        animationManager.Enqueue(animation);
    }
}
