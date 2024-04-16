using Assets.Code.Model.GameEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Model.Skills.PassiveSkills
{
    public class SkillBoom : PassiveSkill
    {
        static int DAMAGE_BASE = 3;
        static int DAMAGE_INCREMENT = 1;
        static float RANGE = 4f;

        public SkillBoom(int level) : base(level) {
            name = "Boom";
        }
        public SkillBoom(SkillBoom other) : base(other) { }
        public override Skill Clone() {
            return new SkillBoom(this);
        }
        public override void AttachTo(Unit u) {
            base.AttachTo(u);
            unit.gameState.gameEventManager.Listen(
                GameEventType.UnitDied,
                e => e.unitSource == unit,
                Handle
            );
        }

        public override string GetDescription() {
            return string.Format($"Explodes when it dies, dealing {GetDamage()} damage to all nearby units.");
        }
        public override string GetIconID() {
            return "boom";
        }
        public int GetDamage() {
            return DAMAGE_BASE + (level - 1) * DAMAGE_INCREMENT;
        }

        bool Handle(GameEvent e) {
            foreach (Unit unit in RangeUtil.GetVisibleOtherUnitsWithinRadius(unit, RANGE)) {
                unit.Damage(GetDamage());
            }
            return false;
        }
    }
}
