using Assets.Code.Model.GameEvents;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Model.Skills
{
    public abstract class Skill : ITooltippableObject {
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
        public virtual bool CanActivate() {
            if (type != SkillType.Active) {
                return false;
            }
            if (RequiresAction() && unit.actions <= 0) {
                return false;
            }
            if (cooldown > 0) {
                return false;
            }
            return true;
        }
        public virtual bool Activate() {
            Debug.Assert(type == SkillType.Active);
            if (!CanActivate()) {
                return false;
            }
            SkillDecision skillDecision = GetDecision();
            if (skillDecision == null) {
                Resolve(null);
            } else {
                bool hasChoices = skillDecision.choices != null && skillDecision.choices.Count > 0;
                bool hasPredicate = skillDecision.predicate != null;
                Debug.Assert(hasChoices || hasPredicate);
                unit.gameState.skillDecision = skillDecision;
            }
            return true;
        }
        public virtual bool WriteUndoHistory() {
            return true;
        }
        public virtual void Resolve(object choice) {
            unit.gameState.skillDecision = null;
            if (WriteUndoHistory()) {
                unit.gameState.gameEventManager.Trigger(new GameEvent() {
                    type = GameEventType.BeforeResolveSkill,
                    actionDetail = new ActionDetail() {
                        type = ActionType.ActivateSkill,
                        skill = this,
                    },
                });
            }
        }
        protected void AfterResolve() {
            if (RequiresAction()) {
                unit.actions--;
            }
            cooldown = GetActivationCooldown();
        }

        public virtual Tooltip GetTooltip() {
            return new Tooltip() {
                header = GetLeveledName(),
                content = GetDescription(),
            };
        }
        public override string ToString() {
            return $"<b>{GetLeveledName()}</b>    {GetDescription()}";
        }
        public string GetLeveledName() {
            return level <= 1 ? name : $"{name} {Util.ToRoman(level)}";
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
