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
    public class ThrowUnitAnimation : AnimationBase {
        static float SPEED = 10f;

        Unit unit;
        Vector3 start, end;
        float height;

        public ThrowUnitAnimation(Unit unit, Vector3 start, Vector3 end, float height) : base(CalculateDuration(start, end, height)) {
            this.unit = unit;
            this.start = start;
            this.end = end;
            this.height = height;
        }
        static float CalculateDuration(Vector3 start, Vector3 end, float height) {
            float maxHeight = (start.y + end.y) / 2 + height;
            float unscaledTime = FreefallTime(maxHeight - start.y) + FreefallTime(maxHeight - end.y);
            return unscaledTime / SPEED;
        }
        static float FreefallTime(float x) {
            return Mathf.Sqrt(2 * x);
        }

        public override void Update() {
            base.Update();
            float t = time.x / time.y;
            unit.MoveTo(CalculateParabolaPoint(start, end, height, t));
        }

        public static Vector3 CalculateParabolaPoint(Vector3 start, Vector3 end, float height, float t) {
            // courtesy of jim3878: https://discussions.unity.com/t/move-from-a-to-b-using-parabola-with-or-without-itween/209654/3
            float x = Mathf.Lerp(start.x, end.x, t);
            float z = Mathf.Lerp(start.z, end.z, t);
            float y = start.y + ((end.y - start.y)) * t + height * (1 - (Mathf.Abs(0.5f - t) / 0.5f) * (Mathf.Abs(0.5f - t) / 0.5f));
            return new Vector3(x, y, z);
        }

        public override bool IsUnitAnimating(Unit unit) {
            return unit == this.unit;
        }
    }
}
