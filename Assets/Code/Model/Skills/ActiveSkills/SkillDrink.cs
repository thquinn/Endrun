using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Model.Skills.ActiveSkills
{
    public class SkillDrink : ActiveSkill {
        static int DAMAGE_BASE = 1;
        static int DAMAGE_INCREMENT = 1;
        static float RANGE = Constants.COMBAT_MELEE_RADIUS;

        public SkillDrink(int level) : base(level) {
            name = "Drink";
        }
        public SkillDrink(SkillDrink other) : base(other) { }
        public override Skill Clone() {
            return new SkillDrink(this);
        }

        public override string GetDescription() {
            return string.Format($"Attack an adjacent enemy for {GetDamage()} damage. If fatal, your Melee skill gets +1 permanently.");
        }
        public override string GetIconID() {
            return "drink";
        }

        public int GetDamage() {
            return DAMAGE_BASE + (level - 1) * DAMAGE_INCREMENT;
        }

        public override object[] GetTargets() {
            return RangeUtil.GetVisibleEnemiesWithinRadius(unit, RANGE);
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
            unit.Attack(target, GetDamage());
            if (unit.dead) {
                Skill melee = unit.skills.First(u => u is SkillMeleeAttack);
                if (melee != null) {
                    melee.level++;
                }
            }
            AfterResolve();
        }
    }
}
