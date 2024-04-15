using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Model.Skills
{
    public class FakeSkillSummon : ActiveSkill {
        static float MIN_RANGE = Constants.COMBAT_MELEE_RADIUS;
        static float MAX_RANGE = Constants.COMBAT_SUMMON_RADIUS;

        public UnitTemplate template;

        public FakeSkillSummon(Unit unit, UnitTemplate template) : base(-1) {
            name = $"Summon {template.name}";
            this.unit = unit;
            this.template = template;
        }
        public FakeSkillSummon(FakeSkillSummon other) : base(other) { }
        public override Skill Clone() {
            return new FakeSkillSummon(this);
        }

        public override int GetManaCost() {
            return template.focusCost;
        }
        public override string GetDescription() {
            return string.Format($"Summon an ally. Requires <b>{template.focusCost}</b> mana and focus.");
        }
        public override string GetIconID() {
            return "summon";
        }

        public override bool CanActivate() {
            int focusLeft = unit.gameState.maxFocus - unit.gameState.GetTotalAllyFocus();
            return base.CanActivate() && focusLeft >= template.focusCost;
        }

        public override SkillDecision GetDecision() {
            return new SkillDecision(this,
                                     "test",
                                     RangePreviewType.Sphere,
                                     MAX_RANGE,
                                     RangeUtil.GetRangePredicate(unit, MIN_RANGE, MAX_RANGE),
                                     RangePreviewType.Ring,
                                     1f);
        }
        public override void Resolve(object choice) {
            base.Resolve(choice);
            Vector3 position = (Vector3) choice;
            unit.gameState.AddUnit(new Unit(unit.gameState, UnitControlType.Ally, position, template));
            AfterResolve();
        }
    }
}
