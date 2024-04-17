using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Model.Skills.ActiveSkills
{
    public class SkillPulse : ActiveSkill {
        static int DAMAGE_BASE = 2;
        static int DAMAGE_INCREMENT = 1;
        static float RANGE_BASE = Constants.COMBAT_MELEE_RADIUS;
        static float RANGE_INCREMENT = 1;

        public SkillPulse(int level) : base(level) {
            name = "Pulse";
        }
        public SkillPulse(SkillPulse other) : base(other) { }
        public override Skill Clone() {
            return new SkillPulse(this);
        }

        public override string GetDescription() {
            return string.Format($"Attack self and all nearby enemies for {GetDamage()} damage.");
        }
        public override string GetIconID() {
            return "pulse";
        }

        public int GetDamage() {
            return DAMAGE_BASE + (level - 1) * DAMAGE_INCREMENT;
        }
        public float GetRange() {
            return RANGE_BASE + (level - 1) * RANGE_INCREMENT;
        }

        public override object[] GetTargets() {
            return RangeUtil.GetVisibleEnemiesWithinRadius(unit, GetRange());
        }
        public override void Resolve(object choice) {
            base.Resolve(choice);
            unit.Damage(GetDamage());
            foreach (object o in GetTargets()) {
                unit.Attack(o as Unit, GetDamage());
            }
            AfterResolve();
        }
    }
}
