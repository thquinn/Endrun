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
        Unit unit;
        NavMeshPath navMeshPath;

        public MoveAnimation(Unit unit, NavMeshPath navMeshPath) : base(CalculateDuration(navMeshPath)) {
            this.unit = unit;
            this.navMeshPath = navMeshPath;
        }
        static float CalculateDuration(NavMeshPath navMeshPath) {
            return NavMeshUtil.GetPathLength(navMeshPath) * .1f;
        }

        public override void Update() {
            base.Update();
            unit.position = NavMeshUtil.GetPointAlongPath(navMeshPath, time.x / time.y);
        }

        public override bool IsUnitAnimating(Unit unit) {
            return unit == this.unit;
        }
    }
}
