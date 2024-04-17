using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Model.Skills.ActiveSkills
{
    public class SkillAccelerate : ActiveSkill {
        static int TICKS_BASE = 10;
        static int TICKS_INCREMENT = 2;
        static int TICK_COST_BASE = 5;
        static int TICK_COST_DECREMENT = 1;
        static float RANGE_BASE = 5;
        static float RANGE_INCREMENT = 1;

        public SkillAccelerate(int level) : base(level) {
            name = "Accelerate";
        }
        public SkillAccelerate(SkillAccelerate other) : base(other) { }
        public override Skill Clone() {
            return new SkillAccelerate(this);
        }

        public override string GetDescription() {
            return string.Format($"Reduce an ally's wait time by {GetTicks()}.");
        }
        public override string GetIconID() {
            return "accelerate";
        }
        public override bool PlayerOnly() {
            return true;
        }

        public int GetTicks() {
            return TICKS_BASE + (level - 1) * TICKS_INCREMENT;
        }
        public override int GetTickCost() {
            return Mathf.Max(0, TICK_COST_BASE + (level - 1) * -TICK_COST_DECREMENT);
        }
        public float GetRange() {
            return RANGE_BASE + (level - 1) * RANGE_INCREMENT;
        }

        public override object[] GetTargets() {
            return RangeUtil.GetVisibleAlliesWithinRadius(unit, GetRange());
        }
        public override SkillDecision GetDecision() {
            return new SkillDecision(this,
                                     "test",
                                     RangePreviewType.Ring,
                                     GetRange(),
                                     GetTargets());
        }
        public override void Resolve(object choice) {
            base.Resolve(choice);
            Unit target = choice as Unit;
            target.SetTicksAndPush(Mathf.Max(1, target.ticksUntilTurn - GetTicks()));
            AfterResolve();
        }
    }
}
