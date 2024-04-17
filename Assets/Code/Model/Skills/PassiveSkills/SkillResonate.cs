using Assets.Code.Model.GameEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Model.Skills.PassiveSkills
{
    public class SkillResonate : PassiveSkill
    {
        static int MULTIPLIER_BASE = 1;
        static int MULTIPLIER_INCREMENT = 1;

        public SkillResonate(int level) : base(level) {
            name = "Resonate";
        }
        public SkillResonate(SkillResonate other) : base(other) { }
        public override Skill Clone() {
            return new SkillResonate(this);
        }
        public override void AttachTo(Unit u) {
            base.AttachTo(u);
            unit.gameState.gameEventManager.Listen(
                GameEventType.ManaOverflow,
                null,
                Handle
            );
        }

        public override string GetDescription() {
            int multiplier = GetMultiplier();
            return multiplier == 1 ?
                $"Whenever you gain excess mana, deal that much damage to all enemies." :
                $"Whenever you gain excess mana, deal {multiplier}x that much damage to all enemies.";
        }
        public override string GetIconID() {
            return "resonate";
        }
        public override bool PlayerOnly() {
            return true;
        }
        public int GetMultiplier() {
            return MULTIPLIER_BASE + (level - 1) * MULTIPLIER_INCREMENT;
        }

        bool Handle(GameEvent e) {
            int damage = Mathf.RoundToInt(e.amount) * GetMultiplier();
            foreach (Unit enemy in RangeUtil.GetEnemies(unit)) {
                enemy.Damage(damage);
            }
            return false;
        }
    }
}
