using Assets.Code.Model.GameEvents;
using Assets.Code.Model.Skills;
using Assets.Code.Model.Skills.ActiveSkills;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Code.Model
{
    public class GameState {
        static UnitTemplate TEMPLATE_SUMMONER = new UnitTemplate("Summoner", 8, 6, 0);
        static UnitTemplate TEMPLATE_SUMMON = new UnitTemplate("Summon", 7, 8, 3, new SkillMeleeAttack(1), new SkillArrow(1));
        static UnitTemplate TEMPLATE_MELEE_ENEMY = new UnitTemplate("Enemy", 7, 8, 3, new SkillMeleeAttack(1));

        public GameEventManager gameEventManager;
        public Random.State enemyAIState;
        public List<UnitTemplate> summonTemplates;
        public List<Chunk> chunks;
        public List<Unit> units;
        public int maxFocus;
        public Vector2Int mana;
        public SkillDecision skillDecision;

        public GameState() {
            gameEventManager = new GameEventManager();
            enemyAIState = Random.state;
            summonTemplates = new List<UnitTemplate>();
            chunks = new List<Chunk>();
            chunks.Add(new Chunk(0, Vector3.zero, false, false));
            units = new List<Unit>();
            maxFocus = 10;
            mana = new Vector2Int(10, 10);
            AddUnit(new Unit(this, UnitControlType.Summoner, new Vector3(0, 2.5f, -8), TEMPLATE_SUMMONER));
            AddUnit(new Unit(this, UnitControlType.Ally, new Vector3(0, -1.5f, 8), TEMPLATE_SUMMON));
            AddUnit(new Unit(this, UnitControlType.Enemy, new Vector3(4, -1.5f, 8), TEMPLATE_MELEE_ENEMY));
        }
        public void AddUnit(Unit unit) {
            if (units.Count == 0) {
                units.Add(unit);
                return;
            }
            for (int i = 1; i < units.Count; i++) {
                if (units[i].ticksUntilTurn > i) {
                    unit.ticksUntilTurn = i;
                    units.Insert(i, unit);
                    return;
                }
            }
            unit.ticksUntilTurn = units.Count;
            units.Add(unit);
            SortUnits();
        }
        public void RemoveUnit(Unit unit) {
            units.Remove(unit);
            SortUnits();
        }
        public void SortUnits() {
            units.Sort((u1, u2) => u1.ticksUntilTurn - u2.ticksUntilTurn);
        }
        public void EndTurn() {
            SortUnits();
            int tickDecrement = units[0].ticksUntilTurn;
            foreach (Unit unit in units) {
                unit.ticksUntilTurn -= tickDecrement;
            }
        }

        public int GetTotalAllyFocus() {
            return units.Where(u => u.playerControlled).Sum(u => u.focusCost);
        }
    }
}
