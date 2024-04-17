using Assets.Code.Animation;
using Assets.Code.Model.GameEvents;
using Assets.Code.Model.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Code.Model
{
    public class Unit : ITooltippableObject {
        static int ID = 0;

        public GameState gameState;
        public int id;
        public bool isSummoner;
        public bool playerControlled;
        public Vector3 position;
        public int ticksUntilTurn, accumulatedTicks;
        public bool dead;
        // From template:
        public List<Skill> skills;
        public string name;
        public string iconID;
        public int focusCost;
        public Vector2Int hp;
        public Vector2 movement;
        public int actions;
        public int movesThisTurn;

        public Unit(GameState gameState, UnitControlType type, Vector3 position, UnitTemplate template) {
            this.gameState = gameState;
            id = ID++;
            isSummoner = type == UnitControlType.Summoner;
            playerControlled = type != UnitControlType.Enemy;
            this.position = position;
            // Copy template.
            skills = new List<Skill>();
            foreach (Skill skill in template.skills) {
                Skill copy = skill.Clone();
                copy.AttachTo(this);
                skills.Add(copy);
            }
            name = template.name;
            iconID = template.iconID;
            focusCost = template.focusCost;
            hp = new Vector2Int(template.hp, template.hp);
            movement = new Vector2(template.movement, template.movement); ;
            actions = 1;
        }

        public void StartTurn() {
            movement.x = movement.y;
            movesThisTurn = 0;
            actions = 1;
        }

        public float XZDistanceTo(Unit other) {
            return Vector2.Distance(new Vector2(position.x, position.z), new Vector2(other.position.x, other.position.z));
        }
        public float SphericalDistanceTo(Unit other) {
            return (other.position - position).magnitude;
        }
        public bool CanShootWithinConicalDistance(Unit other, float coneRadius) {
            float heightAdvantage = position.y - other.position.y;
            float multiplier = Math.Max(0, 1 + heightAdvantage * Constants.COMBAT_CONE_RANGE_MULTIPLIER);
            float xzDistance = XZDistanceTo(other);
            return xzDistance < coneRadius * multiplier;
        }

        public void MoveTo(Vector3 to) {
            Vector3 from = position;
            position = to;
            gameState.gameEventManager.Trigger(new GameEvent() {
                type = GameEventType.MovementSegment,
                unitSource = this,
                actionDetail = new ActionDetail() {
                    positions = new Vector3[] { from, to },
                },
            });
        }
        public void Move(NavMeshPath path) {
            gameState.gameEventManager.Trigger(new GameEvent() {
                type = GameEventType.BeforeMove,
                unitSource = this,
            });
            float length = NavMeshUtil.GetPathLength(path);
            if (length >= movement.x - 1.5f) {
                length = movement.x;
            }
            movement.x -= length;
            movesThisTurn++;
            GameStateManagerScript.instance.EnqueueAnimation(new MoveAnimation(this, path, length));
        }
        public void Attack(Unit target, int amount) {
            if (dead) return;
            target.Damage(amount);
        }
        public void Damage(int amount) {
            if (dead) return;
            amount = Mathf.Min(hp.x, amount);
            hp.x -= amount;
            gameState.gameEventManager.Trigger(new GameEvent() {
                type = GameEventType.Damage,
                unitTarget = this,
                amount = amount,
            });
            if (hp.x == 0) {
                Die();
                gameState.RemoveDeadUnits();
            }
        }
        public void Die() {
            if (dead) return;
            dead = true;
            if (!playerControlled) {
                gameState.manaCrystals.Add(new ManaCrystal(gameState, position));
                // TODO: What if it died in midair or something?
            }
            gameState.gameEventManager.Trigger(new GameEvent() {
                type = GameEventType.UnitDied,
                unitSource = this,
            });
            gameState.gameEventManager.Unregister(this);
            foreach (Skill skill in skills) {
                gameState.gameEventManager.Unregister(skill);
            }
        }
        public void Heal(int amount) {
            if (dead) return;
            amount = Mathf.Min(hp.y - hp.x, amount);
            hp.x += amount;
        }
        public void GainMaxHP(int amount) {
            if (dead) return;
            hp.y += amount;
            Heal(amount);
        }
        public void EndTurn() {
            SetTicks(accumulatedTicks + Constants.BALANCE_BASE_TURN_TICKS);
            accumulatedTicks = 0;
            gameState.EndTurn();
            gameState.gameEventManager.Trigger(new GameEvent() {
                type = GameEventType.TurnStart,
            });
            gameState.units[0].StartTurn();
        }
        void SetTicks(int desiredTicks) {
            // Sets ticksUntilTurn to the desired number, unless another unit already has that exact value.
            // In that case, increment desiredTicks until we find an untied value.
            while (gameState.units.Any(u => u.ticksUntilTurn == desiredTicks)) {
                desiredTicks++;
            }
            ticksUntilTurn = desiredTicks;
        }
        public void SetTicksAndPush(int desiredTicks) {
            // Sets ticksUntilTurn to the desired number, and pushes other units down the order in case of a tie.
            gameState.units.Remove(this);
            int tickCheck = desiredTicks;
            List<Unit> unitsToIncrement = new List<Unit>();
            foreach (Unit u in gameState.units) {
                if (u.ticksUntilTurn < tickCheck) continue;
                if (u.ticksUntilTurn == tickCheck) {
                    tickCheck++;
                    unitsToIncrement.Add(u);
                }
                if (u.ticksUntilTurn > ticksUntilTurn) break;
            }
            foreach (Unit u in unitsToIncrement) {
                u.ticksUntilTurn++;
            }
            ticksUntilTurn = desiredTicks;
            gameState.units.Add(this);
            gameState.SortUnits();
        }

        public Tooltip GetTooltip() {
            string allegianceString = isSummoner ? "Summoner" : playerControlled ? "Summon" : "Enemy";
            string hpString = $"{hp.x}/{hp.y} HP";
            string movementString = $"{movement.y}m move";
            List<string> contentStrings = new List<string>() { allegianceString, hpString, movementString };
            contentStrings.AddRange(skills.Select(s => s.ToString()));
            string content = string.Join('\n', contentStrings);
            return new Tooltip() {
                header = name,
                content = content,
            };
        }
    }

    public enum UnitControlType {
        Summoner, Ally, Enemy
    }
}
