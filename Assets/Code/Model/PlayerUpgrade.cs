using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Model
{
    public class PlayerUpgrade
    {
        public PlayerUpgradeStat stat;
        public UnitTemplate templateNew;
        public UnitTemplateUpgrade templateToUpgrade;

        public PlayerUpgrade(GameState state) {
            float typeSelector = Random.value;
            if (typeSelector < .2f) {
                stat = PlayerUpgradeStat.HP;
            }
            else if (typeSelector < .3f) {
                stat = PlayerUpgradeStat.Movement;
            }
            else {
                float upgradeTemplateChance = Mathf.Sqrt(state.summonTemplates.Count) / 2f;
                if (Random.value < upgradeTemplateChance) {
                    templateToUpgrade = new UnitTemplateUpgrade(Util.ChooseRandom(state.summonTemplates));
                }
                else {
                    templateNew = Util.ChooseRandom(Balance.PLAYER_SPECIAL_TEMPLATES.Where(t => !state.summonTemplates.Any(t2 => t.name == t2.name)).ToArray());
                    int upgrades = state.summonTemplates.Min(t => t.timesUpgraded);
                    for (int i = 0; i < upgrades; i++) {
                        new UnitTemplateUpgrade(templateNew).Apply();
                    }
                }
            }
        }
        public void Apply(GameState state) {
            if (stat == PlayerUpgradeStat.HP) {
                state.units.First(u => u.isSummoner).GainMaxHP(Constants.UPGRADE_UNIT_HP);
            }
            else if (stat == PlayerUpgradeStat.Movement) {
                state.units.First(u => u.isSummoner).movement.y += Constants.UPGRADE_UNIT_MOVEMENT;
            }
            else if (stat == PlayerUpgradeStat.Focus) {
                state.maxFocus += Constants.UPGRADE_MAX_FOCUS;
            }
            else if (templateNew != null) {
                state.summonTemplates.Add(templateNew);
            }
            else if (templateToUpgrade != null) {
                templateToUpgrade.Apply();
            }
            UIScript.instance.lastSkillUnit = null;
        }
        public override string ToString() {
            if (stat == PlayerUpgradeStat.HP) {
                return $"Your summoner gains {Constants.UPGRADE_UNIT_HP} max HP.";
            }
            else if (stat == PlayerUpgradeStat.Movement) {
                return $"Your summoner gets {Constants.UPGRADE_UNIT_MOVEMENT}m move.";
            }
            else if (stat == PlayerUpgradeStat.Focus) {
                return $"Your summoner gains {Constants.UPGRADE_MAX_FOCUS} more focus.";
            }
            else if (templateNew != null) {
                Unit templatedUnit = new Unit(GameStateManagerScript.instance.gameState, UnitControlType.Ally, Vector3.zero, templateNew);
                return $"Gain a new summon:\n\n{templatedUnit.GetTooltip()}";
            }
            else if (templateToUpgrade != null) {
                Unit after = new Unit(GameStateManagerScript.instance.gameState, UnitControlType.Ally, Vector3.zero, templateToUpgrade.Preview());
                return $"Upgrade a summon:\n\n{after.GetTooltip()}";
            }
            return "???";
        }
        public override bool Equals(object obj) {
            PlayerUpgrade other = obj as PlayerUpgrade;
            if (other == null) return false;
            if (other.stat != stat) return false;
            if (other.templateNew != null) return other.templateNew.Equals(templateNew);
            if (other.templateToUpgrade != null) return other.templateToUpgrade.Equals(templateToUpgrade);
            return true;
        }
    }

    public enum PlayerUpgradeStat
    {
        None, HP, Movement, Focus
    }
}
