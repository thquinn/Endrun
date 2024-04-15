using Assets.Code.Model.GameEvents;
using Assets.Code.Model.Skills;
using Assets.Code.Model.Skills.ActiveSkills;
using Assets.Code.Model.Skills.PassiveSkills;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Code.Model
{
    public class GameState {
        static UnitTemplate TEMPLATE_SUMMONER = new UnitTemplate("Summoner", "summoner", 7, 6, 0);
        static UnitTemplate TEMPLATE_BRUTE = new UnitTemplate("Brute", "brute", 10, 5, 3, new SkillMeleeAttack(1), new SkillOpportunist(1));
        static UnitTemplate TEMPLATE_MEDIC = new UnitTemplate("Medic", "medic", 6, 6, 3, new SkillHealingTouch(1));
        static UnitTemplate TEMPLATE_SNIPER = new UnitTemplate("Sniper", "sniper", 5, 7, 3, new SkillArrow(1));
        static UnitTemplate TEMPLATE_WARRIOR = new UnitTemplate("Warrior", "warrior", 8, 7, 3, new SkillMeleeAttack(2));


        public GameEventManager gameEventManager;
        public Random.State enemyAIState;
        public List<UnitTemplate> summonTemplates;
        public List<Chunk> chunks;
        public int chunkTicks;
        public List<Unit> units;
        public int maxFocus;
        public Vector2Int mana;
        public SkillDecision skillDecision;

        public GameState() {
            gameEventManager = new GameEventManager();
            enemyAIState = Random.state;
            summonTemplates = new List<UnitTemplate>();
            summonTemplates.AddRange(new UnitTemplate[] { TEMPLATE_BRUTE, TEMPLATE_MEDIC, TEMPLATE_SNIPER, TEMPLATE_WARRIOR });
            chunks = new List<Chunk>();
            chunks.Add(new Chunk(0, false, false));
            chunks.Add(new Chunk(0, true, true));
            chunkTicks = Constants.BALANCE_CHUNK_TIMER;
            units = new List<Unit>();
            maxFocus = 10;
            mana = new Vector2Int(10, 10);
            AddUnit(new Unit(this, UnitControlType.Summoner, new Vector3(0, 2.5f, -8), TEMPLATE_SUMMONER));
            AddUnit(new Unit(this, UnitControlType.Ally, new Vector3(0, -1.5f, 8), TEMPLATE_MEDIC));
            AddUnit(new Unit(this, UnitControlType.Enemy, new Vector3(4, -1.5f, 8), TEMPLATE_BRUTE));
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
        public void RemoveDeadUnits() {
            for (int i = units.Count - 1; i >= 0; i--) {
                if (units[i].dead) {
                    units.RemoveAt(i);
                }
            }
        }
        public Unit GetActiveUnit() {
            return units.Count == 0 ? null : units[0];
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
            chunkTicks -= tickDecrement;
            if (chunkTicks <= 0) {
                ChunkSwap();
            }
        }
        void ChunkSwap() {
            chunks.RemoveAt(0);
            chunks.Add(new Chunk(0, true, true));
            chunkTicks = Constants.BALANCE_CHUNK_TIMER;
        }

        public int GetTotalAllyFocus() {
            return units.Where(u => u.playerControlled).Sum(u => u.focusCost);
        }
    }
}
