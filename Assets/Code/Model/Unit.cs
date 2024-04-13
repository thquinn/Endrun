using Assets.Code.Animation;
using Assets.Code.Model.GameEvents;
using Assets.Code.Model.Traits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Code.Model
{
    public class Unit {
        GameState gameState;
        public bool isSummoner;
        public bool playerControlled;
        public Vector3 position;
        public int ticksUntilTurn;
        // From template:
        public List<Trait> traits;
        public string name;
        public int focusCost;
        public Vector2Int hp;
        public Vector2 movement;

        public Unit(GameState gameState, UnitControlType type, Vector3 position, UnitTemplate template) {
            this.gameState = gameState;
            isSummoner = type == UnitControlType.Summoner;
            playerControlled = type != UnitControlType.Enemy;
            this.position = position;
            // Copy template.
            traits = new List<Trait>(template.traits);
            name = template.name;
            hp = template.hp;
            movement = template.movement;
        }

        public void Move(NavMeshPath path) {
            float length = NavMeshUtil.GetPathLength(path);
            if (length >= movement.x - 1.5f) {
                length = movement.x;
            }
            movement.x -= length;
            GameStateManagerScript.instance.EnqueueAnimation(new MoveAnimation(this, path, length));
        }
        public void EndTurn() {
            movement.x = movement.y;
            SetTicks(10);
            gameState.units.Sort((u1, u2) => u1.ticksUntilTurn - u2.ticksUntilTurn);
            int tickDecrement = gameState.units[0].ticksUntilTurn;
            foreach (Unit unit in gameState.units) {
                unit.ticksUntilTurn -= tickDecrement;
            }
            gameState.gameEventManager.Trigger(new GameEvent() {
                type = GameEventType.TurnStart,
            });
        }
        void SetTicks(int desiredTicks) {
            // Sets ticksUntilTurn to the desired number, unless another unit already has that exact value.
            // In that case, increment desiredTicks until we find an untied value.
            while (gameState.units.Any(u => u.ticksUntilTurn == desiredTicks)) {
                desiredTicks++;
            }
            ticksUntilTurn = desiredTicks;
        }
    }

    public enum UnitControlType {
        Summoner, Ally, Enemy
    }
}
