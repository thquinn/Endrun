using Assets.Code.Model;
using Assets.Code.Model.GameEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Code.Animation
{
    public class MoveAnimation : AnimationBase {
        static float SPEED = 10f;

        Unit unit;
        NavMeshPath navMeshPath;
        float pathLength;

        public MoveAnimation(Unit unit, NavMeshPath navMeshPath, float pathLength) : base(CalculateDuration(navMeshPath)) {
            this.unit = unit;
            this.navMeshPath = navMeshPath;
            this.pathLength = pathLength;
        }
        static float CalculateDuration(NavMeshPath navMeshPath) {
            return NavMeshUtil.GetPathLength(navMeshPath) / SPEED;
        }

        public override void Update() {
            base.Update();
            unit.MoveTo(NavMeshUtil.GetPointAlongPath(navMeshPath, GetEasedPercentage()));
        }
        public override void Finish() {
            unit.MoveTo(navMeshPath.corners[navMeshPath.corners.Length - 1]);
        }

        public override bool IsUnitAnimating(Unit unit) {
            return unit == this.unit;
        }
        public float GetAnimatedMovementRemaining() {
            return unit.movement.x + pathLength * (1 - GetEasedPercentage());
        }

        float GetEasedPercentage() {
            return EasingFunctions.EaseInOutQuad(0, 1, time.x / time.y);
        }
    }
}
