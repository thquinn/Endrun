using Assets.Code.Model.GameEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Model.Skills.PassiveSkills
{
    public class SkillSpin : PassiveSkill
    {
        static int MANA_BASE = 1;
        static int MANA_INCREMENT = 1;

        public SkillSpin(int level) : base(level) {
            name = "Spin";
        }
        public SkillSpin(SkillSpin other) : base(other) { }
        public override Skill Clone() {
            return new SkillSpin(this);
        }
        public override void AttachTo(Unit u) {
            base.AttachTo(u);
            unit.gameState.gameEventManager.Listen(
                GameEventType.LevelStart,
                null,
                Handle
            );
        }

        public override string GetDescription() {
            return string.Format($"Gain {GetMana()} mana at the start of each level.");
        }
        public override string GetIconID() {
            return "spin";
        }
        public int GetMana() {
            return MANA_BASE + (level - 1) * MANA_INCREMENT;
        }

        bool Handle(GameEvent e) {
            unit.gameState.GainMana(GetMana());
            return false;
        }
    }
}
