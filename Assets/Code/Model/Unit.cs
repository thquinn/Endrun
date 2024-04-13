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
        public List<Trait> traits;
        public int ticksUntilTurn;
        public Vector2Int hp;
        public Vector2 movement;

        public Unit(GameState gameState, bool playerControlled, Vector3 position, params Trait[] traits) {
            this.gameState = gameState;
            this.playerControlled = playerControlled;
            this.position = position;
            this.traits = new List<Trait>(traits);
            movement = new Vector2(8, 8);
        }

        public void Move(NavMeshPath path) {
            GameStateManagerScript.instance.EnqueueAnimation(new MoveAnimation(this, path));
            movement.x -= NavMeshUtil.GetPathLength(path);
            if (movement.x < 1f) {
                movement.x = 0;
            }
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
                type = GameEventType.TurnEnd,
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
}
