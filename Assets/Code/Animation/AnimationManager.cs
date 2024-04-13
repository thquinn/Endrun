using Assets.Code.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Animation
{
    public class AnimationManager {
        public List<AnimationBase> animations;

        public AnimationManager() {
            animations = new List<AnimationBase>();
        }

        public void Update() {
            if (animations.Count == 0) return;
            AnimationBase current = animations[0];
            current.Update();
            if (current.IsDone()) {
                current.Finish();
                animations.RemoveAt(0);
            }
        }

        public T GetCurrentOfType<T>() where T : AnimationBase {
            if (animations.Count == 0) return null;
            return animations[0] as T;
        }
        public void Enqueue(AnimationBase animation) {
            animations.Add(animation);
        }

        public bool IsUnitAnimating(Unit unit) {
            return animations.Count > 0 && animations[0].IsUnitAnimating(unit);
        }
        public bool AnyUnitMoving() {
            return animations.Count > 0 && animations[0] is MoveAnimation;
        }
    }
}
