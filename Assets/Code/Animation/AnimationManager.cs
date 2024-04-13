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
                animations.RemoveAt(0);
            }
        }

        public void Enqueue(AnimationBase animation) {
            animations.Add(animation);
        }
    }
}
