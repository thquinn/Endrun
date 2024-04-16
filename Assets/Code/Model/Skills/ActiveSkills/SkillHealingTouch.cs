using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Model.Skills.ActiveSkills
{
    public class SkillHealingTouch : ActiveSkill {
        static int HEAL_BASE = 2;
        static int HEAL_INCREMENT = 2;
        static float RANGE = Constants.COMBAT_MELEE_RADIUS;

        public SkillHealingTouch(int level) : base(level) {
            name = "Healing Touch";
        }
        public SkillHealingTouch(SkillHealingTouch other) : base(other) { }
        public override Skill Clone() {
            return new SkillHealingTouch(this);
        }

        public override string GetDescription() {
            return string.Format($"Heal an adjacent ally for {GetHealing()} HP.");
        }
        public override string GetIconID() {
            return "healing_touch";
        }

        public int GetHealing() {
            return HEAL_BASE + (level - 1) * HEAL_INCREMENT;
        }

        public override object[] GetTargets() {
            return RangeUtil.GetVisibleAlliesWithinRadius(unit, RANGE).Where(u => u.hp.x < u.hp.y).ToArray();
        }
        public override SkillDecision GetDecision() {
            return new SkillDecision(this,
                                     "test",
                                     RangePreviewType.Ring,
                                     RANGE,
                                     GetTargets());
        }
        public override void Resolve(object choice) {
            base.Resolve(choice);
            Unit target = choice as Unit;
            target.Heal(GetHealing());
            AfterResolve();
        }
    }
}
