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
        public GameEventManager gameEventManager;
        public Random.State enemyAIState;
        public List<UnitTemplate> summonTemplates;
        public List<Chunk> chunks;
        public int level, chunkTicks;
        public List<Unit> units;
        public int maxFocus;
        public Vector2Int mana;
        public List<ManaCrystal> manaCrystals;
        public SkillDecision skillDecision;

        public GameState() {
            gameEventManager = new GameEventManager();
            enemyAIState = Random.state;
            summonTemplates = new List<UnitTemplate>();
            summonTemplates = Balance.GetStartingTemplates();
            chunks = new List<Chunk>();
            units = new List<Unit>();
            manaCrystals = new List<ManaCrystal>();
            Balance.Initialize(this);
            chunkTicks = Constants.BALANCE_CHUNK_TIMER;
            maxFocus = 10;
            mana = new Vector2Int(10, 10);
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
                if (units[i].dead && !units[i].isSummoner) {
                    units.RemoveAt(i);
                }
            }
        }
        public Unit GetActiveUnit() {
            return IsGameOver() ? null : units[0];
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
                gameEventManager.Trigger(new GameEvent() {
                    type = GameEventType.LevelEnd,
                });
                AddChunk();
            }
        }
        public void AddChunk() {
            if (chunks.Count >= 2) {
                chunks.RemoveAt(0);
                level++;
            }
            int chunkIndex = GameStateManagerScript.instance.GetRandomChunkIndex();
            chunks.Add(new Chunk(chunkIndex, Random.value < .5f, Random.value < .5f));
            RemoveOldMana();
            chunkTicks = Constants.BALANCE_CHUNK_TIMER;
            gameEventManager.Trigger(new GameEvent() {
                type = GameEventType.LevelStart,
            });
        }
        void RemoveOldMana() {
            for (int i = manaCrystals.Count - 1; i >= 0; i--) {
                if (manaCrystals[i].IsOffChunks()) {
                    manaCrystals.RemoveAt(i);
                }
            }
        }

        public void GainMana(int amount) {
            int missing = mana.y - mana.x;
            int overflow = Mathf.Max(0, amount - missing);
            mana.x = Mathf.Min(mana.x + amount, mana.y);
            if (overflow > 0) {
                gameEventManager.Trigger(new GameEvent() {
                    type = GameEventType.ManaOverflow,
                    amount = overflow,
                });
            }
        }

        public int GetTotalAllyFocus() {
            return units.Where(u => u.playerControlled).Sum(u => u.focusCost);
        }
        public bool IsGameOver() {
            Unit summoner = units.FirstOrDefault(u => u.isSummoner);
            return summoner == null || summoner.dead;
        }
    }
}
