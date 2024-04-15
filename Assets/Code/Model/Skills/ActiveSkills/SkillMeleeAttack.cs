using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Model.Skills.ActiveSkills
{
    public class SkillMeleeAttack : ActiveSkill {
        static int DAMAGE_BASE = 3;
        static int DAMAGE_INCREMENT = 1;
        static float RANGE = Constants.COMBAT_MELEE_RADIUS;

        public SkillMeleeAttack(int level) : base(level) {
            name = "Melee";
        }
        public SkillMeleeAttack(SkillMeleeAttack other) : base(other) { }
        public override Skill Clone() {
            return new SkillMeleeAttack(this);
        }

        public override string GetDescription() {
            return string.Format($"Attack an adjacent enemy for {GetDamage()} damage.");
        }
        public override string GetIconID() {
            return "melee_attack";
        }

        public int GetDamage() {
            return DAMAGE_BASE + (level - 1) * DAMAGE_INCREMENT;
        }

        public override SkillDecision GetDecision() {
            return new SkillDecision(this,
                                     "test",
                                     RangePreviewType.Ring,
                                     RANGE,
                                     RangeUtil.GetVisibleEnemiesWithinRadius(unit, RANGE));
        }
        public override void Resolve(object choice) {
            base.Resolve(choice);
            Unit target = choice as Unit;
            unit.GetAttacked(target, GetDamage());
            AfterResolve();
        }
    }
}
