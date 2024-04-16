using Assets.Code.Model.Skills;
using Assets.Code.Model.Skills.ActiveSkills;
using Assets.Code.Model.Skills.PassiveSkills;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Code.Model
{
    public class UnitTemplate
    {
        public string name;
        public string iconID;
        public int hp;
        public float movement;
        public int focusCost;
        public List<Skill> skills;
        public int timesUpgraded;

        public UnitTemplate(string name, string iconID, int hp, float movement, int focusCost, params Skill[] skills) {
            this.name = name;
            this.iconID = iconID;
            this.hp = hp;
            this.movement = movement;
            this.focusCost = focusCost;
            this.skills = new List<Skill>(skills);
            timesUpgraded = 0;
        }
        public UnitTemplate(UnitTemplate other) {
            name = other.name;
            iconID = other.iconID;
            hp = other.hp;
            movement = other.movement;
            focusCost = other.focusCost;
            skills = other.skills.Select(s => s.Clone()).ToList();
            timesUpgraded = 0;
        }
    }

    public class UnitTemplateUpgrade
    {
        public UnitTemplate template;
        public UnitTemplateUpgradeStat stat;
        public Skill skillUpgrade, newSkill;

        public UnitTemplateUpgrade(UnitTemplate template) {
            this.template = template;
            float typeSelector = Random.value;
            if (typeSelector < .2f) {
                stat = UnitTemplateUpgradeStat.HP;
            } else if (typeSelector < .3f) {
                stat = UnitTemplateUpgradeStat.Movement;
            } else {
                float upgradeSkillChance = Mathf.Sqrt(template.skills.Count) / 2f;
                if (Random.value < upgradeSkillChance) {
                    skillUpgrade = Util.ChooseRandom(template.skills);
                } else {
                    newSkill = Util.ChooseRandom(UPGRADE_SKILLS.Where(s => !template.skills.Any(s2 => s.name == s2.name)).ToArray()).Clone();
                    newSkill.level = template.skills.Count == 0 ? 0 : template.skills.Min(s => s.level) + 1;
                }
            }
        }

        public UnitTemplate Preview() {
            return ApplyInternal(new UnitTemplate(template));
        }
        public void Apply() {
            ApplyInternal(template);
        }
        UnitTemplate ApplyInternal(UnitTemplate templateCopy) {
            if (stat == UnitTemplateUpgradeStat.HP) {
                templateCopy.hp++;
            }
            else if (stat == UnitTemplateUpgradeStat.Movement) {
                templateCopy.movement += 1.5f;
            }
            else if (skillUpgrade != null) {
                skillUpgrade.level++;
            }
            else if (newSkill != null) {
                templateCopy.skills.Add(newSkill);
            }
            templateCopy.timesUpgraded++;
            if (templateCopy.timesUpgraded % Constants.UNIT_UPGRADES_PER_FOCUS_COST == 0) {
                templateCopy.focusCost++;
            }
            return templateCopy;
        }

        static Skill[] UPGRADE_SKILLS = new Skill[] {
            new SkillAccelerate(1), new SkillArrow(1), new SkillDrink(1), new SkillHealingTouch(1), new SkillPulse(1), new SkillSuplex(1), new SkillTeleport(1),
            new SkillBoom(1), new SkillOpportunist(1), new SkillResonate(1), new SkillSpin(1),
        };
    }
    public enum UnitTemplateUpgradeStat {
        None, HP, Movement
    }
}
