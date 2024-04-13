using Assets.Code.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Animation
{
    public abstract class AnimationBase {
        public Vector2 time;

        public AnimationBase(float duration) {
            time = new Vector2(0, duration);
        }
        public virtual void Update() {
            time.x = Mathf.Min(time.x + Time.deltaTime, time.y);
        }
        public bool IsDone() {
            return time.x >= time.y;
        }
        
        public virtual void Finish() { }
        public virtual bool EndTurnOnFinish() { return false; }
        public virtual bool IsUnitAnimating(Unit unit) { return false; }
    }
}
