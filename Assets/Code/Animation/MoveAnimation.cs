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
        UnitScript unitScript;
        NavMeshPath navMeshPath;

        public MoveAnimation(Unit unit, NavMeshPath navMeshPath) : base(CalculateDuration(navMeshPath)) {
            unitScript = GameStateManagerScript.instance.unitScripts[unit];
            this.navMeshPath = navMeshPath;
        }
        static float CalculateDuration(NavMeshPath navMeshPath) {
            return NavMeshUtil.GetPathLength(navMeshPath) * .1f;
        }

        public override void Update() {
            base.Update();
            unitScript.transform.localPosition = NavMeshUtil.GetPointAlongPath(navMeshPath, time.x / time.y);
        }
    }
}
