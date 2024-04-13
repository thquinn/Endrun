using Assets.Code.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.AI;

namespace Assets.Code.Animation
{
    public class MoveAnimation : AnimationBase {
        static float SPEED = 20f;

        Unit unit;
        NavMeshPath navMeshPath;

        public MoveAnimation(Unit unit, NavMeshPath navMeshPath) : base(CalculateDuration(navMeshPath)) {
            this.unit = unit;
            this.navMeshPath = navMeshPath;
        }
        static float CalculateDuration(NavMeshPath navMeshPath) {
            return NavMeshUtil.GetPathLength(navMeshPath) / SPEED;
        }

        public override void Update() {
            base.Update();
            unit.position = NavMeshUtil.GetPointAlongPath(navMeshPath, time.x / time.y);
        }

        public override void Finish() {
            if (unit.movement.x <= 0) {
                unit.EndTurn();
            }
        }
    }
}
