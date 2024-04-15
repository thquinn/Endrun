using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Model.Skills.ActiveSkills
{
    public class SkillArrow : ActiveSkill {
        static int DAMAGE_BASE = 2;
        static int DAMAGE_INCREMENT = 1;
        static float MIN_RANGE = Mathf.CeilToInt(Constants.COMBAT_MELEE_RADIUS);
        static float MAX_RANGE = 6;
        static float MAX_RANGE_INCREMENT = 1;

        public SkillArrow(int level) : base(level) {
            name = "Arrow";
        }
        public SkillArrow(SkillArrow other) : base(other) { }
        public override Skill Clone() {
            return new SkillArrow(this);
        }

        public override string GetDescription() {
            return string.Format($"Attack an enemy {MIN_RANGE}-{GetMaxRange()}m away for {GetDamage()} damage.");
        }
        public override string GetIconID() {
            return "arrow";
        }

        public int GetDamage() {
            return DAMAGE_BASE + (level - 1) * DAMAGE_INCREMENT;
        }
        public float GetMaxRange() {
            return MAX_RANGE + (level - 1) * MAX_RANGE_INCREMENT;
        }

        public override SkillDecision GetDecision() {
            HashSet<Unit> withinMax = new HashSet<Unit>(RangeUtil.GetVisibleEnemiesWithinCone(unit, GetMaxRange()));
            foreach (Unit unit in RangeUtil.GetVisibleEnemiesWithinRadius(unit, MIN_RANGE)) {
                withinMax.Remove(unit);
            }
            return new SkillDecision(this,
                                     "test",
                                     RangePreviewType.Cone,
                                     MAX_RANGE,
                                     withinMax.ToArray());
        }
        public override void Resolve(object choice) {
            base.Resolve(choice);
            Unit target = choice as Unit;
            unit.GetAttacked(target, GetDamage());
            AfterResolve();
        }
    }
}
