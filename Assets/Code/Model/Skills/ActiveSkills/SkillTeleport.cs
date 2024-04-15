using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Model.Skills.ActiveSkills
{
    public class SkillTeleport : ActiveSkill {
        static int RANGE_BASE = 5;
        static int RANGE_INCREMENT = 3;

        public SkillTeleport(int level) : base(level) {
            name = "Teleport";
        }
        public SkillTeleport(SkillTeleport other) : base(other) { }
        public override Skill Clone() {
            return new SkillTeleport(this);
        }

        public override string GetDescription() {
            return string.Format($"Instantly move to a spot ${GetRange()}m away.");
        }
        public override int GetTickCost() {
            return 5;
        }
        public override string GetIconID() {
            return "teleport";
        }

        public int GetRange() {
            return RANGE_BASE + (level - 1) * RANGE_INCREMENT;
        }

        public override SkillDecision GetDecision() {
            float range = GetRange();
            return new SkillDecision(this,
                                     "test",
                                     RangePreviewType.Sphere,
                                     range,
                                     RangeUtil.GetRangePredicate(unit, 0, range),
                                     RangePreviewType.Ring,
                                     .5f);
        }
        public override void Resolve(object choice) {
            base.Resolve(choice);
            Vector3 position = (Vector3)choice;
            unit.MoveTo(position);
            AfterResolve();
        }
    }
}
