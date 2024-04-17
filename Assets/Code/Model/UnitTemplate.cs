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

        public override bool Equals(object obj) {
            UnitTemplate other = obj as UnitTemplate;
            if (other == null) return false;
            bool statsEqual = other.name == name && other.iconID == iconID && other.hp == hp && other.movement == movement && other.focusCost == focusCost;
            if (!statsEqual) return false;
            if (other.skills.Count != skills.Count) return false;
            bool skillsEqual = other.skills.All(s => skills.Contains(s)) && skills.All(s => other.skills.Contains(s));
            return skillsEqual;
        }
    }

    public class UnitTemplateUpgrade
    {
        public UnitTemplate template;
        public UnitTemplateUpgradeStat stat;
        public Skill skillToUpgrade, skillNew;

        public UnitTemplateUpgrade(UnitTemplate template, bool player) {
            this.template = template;
            float typeSelector = Random.value;
            if (typeSelector < .2f) {
                stat = UnitTemplateUpgradeStat.HP;
            } else if (typeSelector < .3f) {
                stat = UnitTemplateUpgradeStat.Movement;
            } else {
                float upgradeSkillChance = Mathf.Sqrt(template.skills.Count) / 2f;
                if (Random.value < upgradeSkillChance) {
                    skillToUpgrade = Util.ChooseRandom(template.skills);
                } else {
                    skillNew = Util.ChooseRandom(UPGRADE_SKILLS.Where(s => player || !s.PlayerOnly())
                                                               .Where(s => !template.skills.Any(s2 => s.name == s2.name))
                                                               .ToArray()).Clone();
                    skillNew.level = template.skills.Count == 0 ? 0 : template.skills.Min(s => s.level);
                }
            }
        }

        public UnitTemplate Preview() {
            UnitTemplate copy = new UnitTemplate(template);
            ApplyInternal(copy);
            return copy;
        }
        public void Apply() {
            ApplyInternal(template);
        }
        void ApplyInternal(UnitTemplate templateCopy) {
            if (stat == UnitTemplateUpgradeStat.HP) {
                templateCopy.hp += Constants.UPGRADE_UNIT_HP;
            }
            else if (stat == UnitTemplateUpgradeStat.Movement) {
                templateCopy.movement += Constants.UPGRADE_UNIT_MOVEMENT;
            }
            else if (skillToUpgrade != null) {
                templateCopy.skills.First(s => s.name == skillToUpgrade.name).level++;
            }
            else if (skillNew != null) {
                templateCopy.skills.Add(skillNew);
            }
            templateCopy.timesUpgraded++;
            if (templateCopy.timesUpgraded % Constants.UPGRADES_PER_FOCUS_COST_INCREASE == 0) {
                templateCopy.focusCost++;
            }
        }

        public override string ToString() {
            if (stat == UnitTemplateUpgradeStat.HP) {
                return $"+{Constants.UPGRADE_UNIT_HP} max HP";
            }
            else if (stat == UnitTemplateUpgradeStat.Movement) {
                return $"+{Constants.UPGRADE_UNIT_MOVEMENT}m move";
            }
            else if (skillNew != null) {
                return $"New skill:\n\n{skillNew}";
            }
            else if (skillToUpgrade != null) {
                Skill after = skillToUpgrade.Clone();
                after.level++;
                return $"{skillToUpgrade}<line-height=50%>\n\n<align=\"center\">▼</align>\n\n</line-height>{after}";
            }
            return "???";
        }
        public override bool Equals(object obj) {
            UnitTemplateUpgrade other = obj as UnitTemplateUpgrade;
            if (other == null) return false;
            if (!other.template.Equals(template)) return false;
            if (other.stat != stat) return false;
            if (other.skillNew != null) return other.skillNew.Equals(skillNew);
            if (other.skillToUpgrade != null) return other.skillToUpgrade.Equals(skillToUpgrade);
            return true;
        }

        static Skill[] UPGRADE_SKILLS = new Skill[] {
            new SkillAccelerate(1), new SkillArrow(1), new SkillHealingTouch(1), new SkillPulse(1), new SkillSuplex(1), new SkillTeleport(1),
            new SkillBoom(1), new SkillOpportunist(1), new SkillResonate(1), new SkillSpin(1),
        };
    }
    public enum UnitTemplateUpgradeStat {
        None, HP, Movement
    }
}
