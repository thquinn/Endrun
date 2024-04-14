using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Model.Skills
{
    public abstract class Skill {
        public Unit unit;
        public SkillType type;
        public int level;
        public string name;
        public int cooldown;

        public Skill(SkillType type, int level) {
            this.type = type;
            this.level = level;
        }
        public Skill(Skill other) {
            type = other.type;
            level = other.level;
            name = other.name;
        }
        public abstract Skill Clone();
        public virtual void AttachTo(Unit unit) {
            this.unit = unit;
        }

        public abstract string GetDescription();
        public abstract string GetIconID();
        public virtual bool RequiresAction() { return type == SkillType.Active; }
        public virtual int GetActivationCooldown() { return 0; }
        public virtual SkillDecision GetDecision() {
            Debug.Assert(type == SkillType.Active);
            return null;
        }
        public virtual bool Activate() {
            Debug.Assert(type == SkillType.Active);
            if (RequiresAction() && unit.actions <= 0) {
                return false;
            }
            if (cooldown > 0) {
                return false;
            }
            SkillDecision skillDecision = GetDecision();
            Debug.Assert(skillDecision == null || skillDecision.choices.Count > 0);
            if (skillDecision == null) {
                Resolve(null);
            } else {
                unit.gameState.skillDecision = skillDecision;
            }
            return true;
        }
        public virtual void Resolve(object choice) {
            if (RequiresAction()) {
                unit.actions--;
            }
            cooldown = GetActivationCooldown();
            unit.gameState.skillDecision = null;
        }
    }

    public enum SkillType
    {
        Active, Passive
    }

    public abstract class ActiveSkill : Skill
    {
        public ActiveSkill(int level) : base(SkillType.Active, level) { }
        public ActiveSkill(ActiveSkill other) : base(other) { }
    }
    public abstract class PassiveSkill : Skill
    {
        public PassiveSkill(int level) : base(SkillType.Passive, level) { }
        public PassiveSkill(PassiveSkill other) : base(other) { }
    }
}
