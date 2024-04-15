using Assets.Code.Model;
using Assets.Code.Model.Skills;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Code
{
    public static class EnemyAI {
        static float DELAY = .25f;
        public static float wait = 0;

        public static void DecideWithDelay(Unit unit) {
            if (wait == 0) {
                wait = DELAY;
                return;
            } else {
                wait -= Time.deltaTime;
                if (wait <= 0) {
                    wait = 0;
                    StartEnemyRandom(unit);
                    Decide(unit);
                    EndEnemyRandom(unit);
                }
            }
        }
        static void Decide(Unit unit) {
            // Try to use skills.
            foreach (Skill skill in unit.skills) {
                if (skill.CanActivate()) {
                    SkillDecision decision = skill.GetDecision();
                    if (decision.choices.Count > 0) {
                        decision.MakeDecision(decision.choices.First());
                        return;
                    }
                }
            }
            // Find a path to a point where a skill can be used.
            if (unit.movement.x > 0) {
                NavMeshAgent navMeshAgent = GameStateManagerScript.instance.unitScripts[unit].GetComponent<NavMeshAgent>();
                NavMeshPath bestPath = null;
                navMeshAgent.nextPosition = unit.position;
                for (int i = 0; i < 1000; i++) {
                    NavMeshPath path = TryGetRandomMovePath(unit, navMeshAgent);
                    if (path == null) continue;
                    // Could any of my skills be used from this position?
                    bool canUseSkill = false;
                    Vector3 savedPosition = unit.position;
                    unit.position = path.corners[path.corners.Length - 1];
                    foreach (Skill skill in unit.skills) {
                        if (skill.CanActivate()) {
                            SkillDecision decision = skill.GetDecision();
                            if (decision.choices.Count > 0) {
                                canUseSkill = true;
                                bestPath = path;
                                break;
                            }
                        }
                    }
                    unit.position = savedPosition;
                    if (canUseSkill) {
                        break;
                    }
                    bestPath = path;
                }
                if (bestPath != null) {
                    unit.Move(bestPath);
                    return;
                }
            }
            // End turn.
            unit.EndTurn();
        }

        static NavMeshPath TryGetRandomMovePath(Unit unit, NavMeshAgent navMeshAgent) {
            Vector3 randomPoint = unit.position + Random.insideUnitSphere * unit.movement.x;
            NavMeshHit navMeshHit;
            NavMesh.SamplePosition(randomPoint, out navMeshHit, unit.movement.x, NavMesh.AllAreas);
            if (!navMeshHit.hit) {
                return null; 
            }
            NavMeshPath path = new NavMeshPath();
            navMeshAgent.CalculatePath(navMeshHit.position, path);
            float pathLength = NavMeshUtil.GetPathLength(path);
            if (pathLength > unit.movement.x || path.corners.Length < 2) {
                return null;
            }
            return path;
        }

        static Random.State savedRandom;
        static void StartEnemyRandom(Unit unit) {
            savedRandom = Random.state;
            Random.state = unit.gameState.enemyAIState;
        }
        static void EndEnemyRandom(Unit unit) {
            unit.gameState.enemyAIState = Random.state;
            Random.state = savedRandom;
        }
    }
}
