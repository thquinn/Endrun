using Assets.Code.Animation;
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
        public bool isSummoner;
        public bool playerControlled;
        public Vector3 position;
        public List<Trait> traits;
        public int ticksUntilTurn;
        public Vector2Int hp;
        public Vector2 movement;

        public Unit(bool playerControlled, Vector3 position, params Trait[] traits) {
            this.playerControlled = playerControlled;
            this.position = position;
            this.traits = new List<Trait>(traits);
            movement = new Vector2(8, 8);
        }

        public void Move(NavMeshPath path) {
            GameStateManagerScript.instance.EnqueueAnimation(new MoveAnimation(this, path));
            movement.x -= NavMeshUtil.GetPathLength(path);
            if (movement.x < .5f) {
                movement.x = 0;
            }
        }
    }
}
