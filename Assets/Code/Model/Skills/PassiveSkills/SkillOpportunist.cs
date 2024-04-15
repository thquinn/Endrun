using Assets.Code.Model.GameEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Model.Skills.PassiveSkills
{
    public class SkillOpportunist : PassiveSkill
    {
        static int DAMAGE_BASE = 1;
        static int DAMAGE_INCREMENT = 1;

        public SkillOpportunist(int level) : base(level) {
            name = "Opportunist";
        }
        public SkillOpportunist(SkillOpportunist other) : base(other) { }
        public override Skill Clone() {
            return new SkillOpportunist(this);
        }
        public override void AttachTo(Unit u) {
            base.AttachTo(u);
            unit.gameState.gameEventManager.Listen(
                GameEventType.MovementSegment,
                e => e.unitSource.playerControlled != unit.playerControlled,
                Handle
            );
        }

        public override string GetDescription() {
            return string.Format($"Whenever an enemy leaves melee range, deal {GetDamage()} damage to it.");
        }
        public override string GetIconID() {
            return "opportunist";
        }
        public int GetDamage() {
            return DAMAGE_BASE + (level - 1) * DAMAGE_INCREMENT;
        }

        bool Handle(GameEvent e) {
            if (unit.gameState.GetActiveUnit() == unit) return false;
            Vector3 from = e.actionDetail.positions[0];
            Vector3 to = e.actionDetail.positions[1];
            if (RangeUtil.WithinMeleeRange(unit.position, from) && !RangeUtil.WithinMeleeRange(unit.position, to)) {
                e.unitSource.Damage(GetDamage());
            }
            // TODO: limit once per turn
            return false;
        }
    }
}
