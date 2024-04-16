using Assets.Code.Model;
using Assets.Code.Model.Skills.ActiveSkills;
using Assets.Code.Model.Skills.PassiveSkills;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Code
{
    public static class Balance {
        static UnitTemplate TEMPLATE_SUMMONER = new UnitTemplate("Summoner", "summoner", 7, 8, 0);
        static UnitTemplate TEMPLATE_BOMB = new UnitTemplate("Bomb", "bomb", 6, 6, 3, new SkillPulse(1), new SkillBoom(1));
        static UnitTemplate TEMPLATE_BRUTE = new UnitTemplate("Brute", "brute", 9, 7, 3, new SkillMeleeAttack(1), new SkillOpportunist(1));
        static UnitTemplate TEMPLATE_CRYSTAL = new UnitTemplate("Crystal", "crystal", 4, 7, 2, new SkillSpin(1), new SkillResonate(1));
        static UnitTemplate TEMPLATE_MEDIC = new UnitTemplate("Medic", "medic", 6, 9, 3, new SkillHealingTouch(1));
        static UnitTemplate TEMPLATE_MERIDIAN = new UnitTemplate("Meridian", "meridian", 5, 7, 3, new SkillAccelerate(1), new SkillTeleport(1));
        static UnitTemplate TEMPLATE_SNIPER = new UnitTemplate("Sniper", "sniper", 5, 10, 3, new SkillArrow(1));
        static UnitTemplate TEMPLATE_VAMPIRE = new UnitTemplate("Vampire", "vampire", 5, 5, 4, new SkillMeleeAttack(1), new SkillDrink(1));
        static UnitTemplate TEMPLATE_WARRIOR = new UnitTemplate("Warrior", "warrior", 8, 10, 3, new SkillMeleeAttack(2), new SkillSuplex(1));
        static UnitTemplate[] PLAYER_SPECIAL_TEMPLATES = new UnitTemplate[] { TEMPLATE_BOMB, TEMPLATE_BRUTE, TEMPLATE_CRYSTAL, TEMPLATE_MEDIC, TEMPLATE_MERIDIAN, TEMPLATE_VAMPIRE };
        static Dictionary<UnitTemplate, float> ENEMY_TEMPLATE_WEIGHTS = new Dictionary<UnitTemplate, float>() {
            { TEMPLATE_BOMB, .5f },
            { TEMPLATE_BRUTE, .75f },
            { TEMPLATE_MEDIC, .75f },
            { TEMPLATE_SNIPER, 1f },
            { TEMPLATE_WARRIOR, 1f },
        };

        public static List<UnitTemplate> GetStartingTemplates() {
            return new List<UnitTemplate>(new UnitTemplate[] { TEMPLATE_WARRIOR, TEMPLATE_SNIPER });
        }
        public static void Initialize(GameState gameState) {
            gameState.chunks.Add(new Chunk(0, false, false));
            gameState.AddUnit(new Unit(gameState, UnitControlType.Summoner, new Vector3(0, 2.5f, -8), TEMPLATE_SUMMONER));
            gameState.AddChunk();
        }

        public static void InitializeNewChunk(GameState gameState) {
            Queue<Vector3> spawnPoints = new Queue<Vector3>(GetPointsOnChunk(gameState.chunks[1], 20));
            // Enemies.
            UnitTemplate[] enemyTemplates;
            if (gameState.level == 0) {
                enemyTemplates = new UnitTemplate[] { TEMPLATE_WARRIOR };
            } else {
                enemyTemplates = GetEnemyTemplates(Mathf.FloorToInt(Random.Range(2f, 3.25f)));
            }
            int upgradeBudget = gameState.level;
            for (int i = 0; i < upgradeBudget; i++) {
                UnitTemplate enemyTemplate = Util.ChooseRandom(enemyTemplates);
                // Pick a non-new-skill upgrade, if possible.
                UnitTemplateUpgrade[] upgrades = new UnitTemplateUpgrade[] { new UnitTemplateUpgrade(enemyTemplate), new UnitTemplateUpgrade(enemyTemplate) };
                UnitTemplateUpgrade nonNewSkill = upgrades.FirstOrDefault(u => u.newSkill == null);
                UnitTemplateUpgrade upgrade = nonNewSkill ?? upgrades[0];
                upgrade.Apply();
            }
            float enemyBudget = 3 + 2 * gameState.level;
            for (int i = 0; i < 100 && enemyBudget > 0 && spawnPoints.Count > 0; i++) {
                UnitTemplate enemyTemplate = Util.ChooseRandom(enemyTemplates);
                float cost = 1f + enemyTemplate.timesUpgraded * .33f;
                if (enemyBudget < cost) continue;
                gameState.AddUnit(new Unit(gameState, UnitControlType.Enemy, spawnPoints.Dequeue(), enemyTemplate));
                enemyBudget -= cost;
            }
            // Mana crystals.
            for (int i = 0; i < Constants.SPAWN_MANA_CRYSTALS_PER_LEVEL && spawnPoints.Count > 0; i++) {
                gameState.manaCrystals.Add(new ManaCrystal(gameState, spawnPoints.Dequeue()));
            }
        }

        //
        // Helpers.
        //
        static List<Vector3> GetPointsOnChunk(Chunk chunk, int n) {
            List<Vector3> points = new List<Vector3>();
            for (int i = 0; i < 1000; i++) {
                Vector3 point = GetPointOnChunk(chunk);
                if (point.x == float.PositiveInfinity) continue;
                if (points.Any(p => Vector3.Distance(p, point) < Constants.SPAWN_MIN_DISTANCE_BETWEEN)) continue;
                points.Add(point);
                if (points.Count >= n) break;
            }
            return points;
        }
        static Vector3 GetPointOnChunk(Chunk chunk) {
            Vector3 position = chunk.position + new Vector3(Random.Range(-9f, 9f), 0, Random.Range(-9, 9f));
            NavMeshHit navMeshHit;
            NavMesh.SamplePosition(position, out navMeshHit, 10f, NavMesh.AllAreas);
            if (navMeshHit.hit) {
                Debug.DrawLine(position, navMeshHit.position, Color.white, 1000);
            } else {
                Debug.DrawLine(position, position + Vector3.up, Color.red, 1000);
            }
            return navMeshHit.position;
        }

        static UnitTemplate[] GetEnemyTemplates(int n) {
            List<UnitTemplate> enemyTemplates = new List<UnitTemplate>();
            float enemyTemplateWeightsSum = ENEMY_TEMPLATE_WEIGHTS.Values.Sum();
            while (enemyTemplates.Count < n) {
                float selector = Random.Range(0, enemyTemplateWeightsSum);
                foreach (var kvp in ENEMY_TEMPLATE_WEIGHTS) {
                    selector -= kvp.Value;
                    if (selector < 0) {
                        if (!enemyTemplates.Any(t => t.name == kvp.Key.name)) {
                            enemyTemplates.Add(kvp.Key);
                        }
                        break;
                    }
                }
            }
            return enemyTemplates.ToArray();
        }
    }
}
