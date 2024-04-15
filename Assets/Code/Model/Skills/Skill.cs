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
        public virtual int GetTickCost() { return 0; }
        public virtual int GetManaCost() { return 0; }
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
            if (unit.gameState.mana.x < GetManaCost() && unit.playerControlled) {
                return false;
            }
            if (cooldown > 0) {
                return false;
            }
            if (GameStateManagerScript.instance.animationManager.IsAnythingAnimating()) {
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
            unit.accumulatedTicks += GetTickCost();
            if (unit.playerControlled) {
                unit.gameState.mana.x -= GetManaCost();
            }
            cooldown = GetActivationCooldown();
        }

        public virtual Tooltip GetTooltip() {
            int ticks = GetTickCost();
            int actions = RequiresAction() ? 1 : 0;
            int mana = GetManaCost();
            string costString = "";
            if (actions > 0 || ticks > 0 || mana > 0) {
                string actionsIcons = string.Concat(Enumerable.Repeat("<sprite name=\"cost_action\">", actions));
                string actionsWords = actions == 0 ? "" : actions == 1 ? "one action" : $"{actions} actions";
                string ticksIcons = ticks == 0 ? "" : $"{ticks}<sprite name=\"cost_tick\">";
                string ticksWords = ticks == 0 ? "" : ticks == 1 ? "one tick" : $"{ticks} ticks";
                string manaIcons = string.Concat(Enumerable.Repeat("<sprite name=\"cost_mana\">", mana));
                string manaWords = mana == 0 ? "" : mana == 1 ? "one mana" : $"{mana} mana";
                string costIcons, costWords;
                costIcons = string.Join(" ", new string[] { actionsIcons, ticksIcons, manaIcons }.Where(s => s.Length > 0));
                costWords = string.Join(", ", new string[] { actionsWords, ticksWords, manaWords }.Where(s => s.Length > 0));
                costString = $"cost: {costIcons}\n<size=66%>({costWords})";
            }

            return new Tooltip() {
                header = GetLeveledName(),
                content = GetDescription(),
                upperRight = costString,
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
