using Assets.Code.Model;
using Assets.Code.Model.Skills;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UIScript : MonoBehaviour
{
    static KeyCode[] SKILL_HOTKEYS = new KeyCode[] { KeyCode.Z, KeyCode.X, KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4,
                                                     KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0 };

    public GameObject prefabUILeftUnit, prefabSkillButton, prefabUIUnitTurn;

    public GameStateManagerScript gameStateManagerScript;
    public RectTransform rtLeftUnits, rtSkillBar, rtTurnOrderList, rtChunkTimer;
    public CanvasGroup canvasGroupSkillBar;
    public TextMeshProUGUI tmpChunkTimerTicks, tmpChunkTimerLabel;

    Dictionary<Unit, UILeftUnitScript> leftUnitScripts;
    Dictionary<Unit, UIUnitTurnScript> unitTurnScripts;

    Unit lastSkillUnit;
    GameState lastTurnGameState;
    float vChunkTimer;

    void Start() {
        leftUnitScripts = new Dictionary<Unit, UILeftUnitScript>();
        unitTurnScripts = new Dictionary<Unit, UIUnitTurnScript>();
    }

    void Update() {
        SyncLeftUnits();
        SyncSkillBar();
        SyncTurnOrder();
        SyncChunkTimer();
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
        var nulls = leftUnitScripts.Keys.Where(u => !units.Contains(u)).ToArray();
        foreach (Unit unit in nulls) {
            Destroy(leftUnitScripts[unit].gameObject);
            leftUnitScripts.Remove(unit);
        }
    }

    void SyncSkillBar() {
        Unit activeUnit = gameStateManagerScript.GetActiveUnit();
        if (!activeUnit.playerControlled) activeUnit = null;
        if (lastSkillUnit != activeUnit) {
            foreach (Transform child in rtSkillBar) {
                Destroy(child.gameObject);
            }
            if (activeUnit != null) {
                List<Skill> skills = new List<Skill>();
                skills.Add(new FakeSkillUndo(activeUnit));
                skills.Add(new FakeSkillEndTurn(activeUnit));
                if (activeUnit.isSummoner) {
                    foreach (UnitTemplate template in activeUnit.gameState.summonTemplates) {
                        skills.Add(new FakeSkillSummon(activeUnit, template));
                    }
                }
                skills.AddRange(activeUnit.skills);
                for (int i = 0; i < skills.Count; i++) {
                    Instantiate(prefabSkillButton, rtSkillBar).GetComponent<UISkillButtonScript>().Init(skills[i], SKILL_HOTKEYS[i]);
                }
            }
            lastSkillUnit = activeUnit;
        }
        float alpha = 0;
        if (activeUnit != null) {
            alpha = gameStateManagerScript.animationManager.IsAnythingAnimating() ? .5f : 1;
        }
        canvasGroupSkillBar.alpha = alpha;
    }

    void SyncTurnOrder() {
        bool undid = false;
        if (GameStateManagerScript.instance.gameState != lastTurnGameState) {
            undid = true;
            lastTurnGameState = GameStateManagerScript.instance.gameState;
        }
        List<Unit> units = gameStateManagerScript.gameState.units;
        foreach (Unit unit in units) {
            if (!unitTurnScripts.ContainsKey(unit)) {
                UIUnitTurnScript uiUnitTurnScript = Instantiate(prefabUIUnitTurn, rtTurnOrderList).GetComponent<UIUnitTurnScript>();
                uiUnitTurnScript.Init(unit, undid);
                unitTurnScripts[unit] = uiUnitTurnScript;
            }
        }
        var nulls = unitTurnScripts.Keys.Where(u => !units.Contains(u)).ToArray();
        foreach (Unit unit in nulls) {
            Destroy(unitTurnScripts[unit].gameObject);
            unitTurnScripts.Remove(unit);
        }
    }

    void SyncChunkTimer() {
        int ticks = gameStateManagerScript.gameState.chunkTicks;
        tmpChunkTimerTicks.text = ticks.ToString();
        tmpChunkTimerLabel.text = ticks == 1 ? "tick\nremaining" : "ticks\nremaining";
        float targetX = gameStateManagerScript.gameState.units.Count * -UIUnitTurnScript.SPACING + 20;
        float x = Mathf.SmoothDamp(rtChunkTimer.anchoredPosition.x, targetX, ref vChunkTimer, .2f);
        rtChunkTimer.anchoredPosition = new Vector2(x, 0);
    }
}
