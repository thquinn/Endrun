using Assets.Code.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Code.Model.Skills.ActiveSkills
{
    public class SkillSuplex : ActiveSkill {
        static int DAMAGE_BASE = 1;
        static int DAMAGE_INCREMENT = 1;
        static float THROW_DISTANCE_BASE = 2f;
        static float THROW_DISTANCE_INCREMENT = 1f;
        static float RANGE = Constants.COMBAT_MELEE_RADIUS;

        public SkillSuplex(int level) : base(level) {
            name = "Suplex";
        }
        public SkillSuplex(SkillSuplex other) : base(other) { }
        public override Skill Clone() {
            return new SkillSuplex(this);
        }

        public override string GetDescription() {
            return string.Format($"Throw an enemy behind you and deal {GetDamage()} damage.");
        }
        public override string GetIconID() {
            return "suplex";
        }
        public override int GetTickCost() {
            return 3;
        }

        public int GetDamage() {
            return DAMAGE_BASE + (level - 1) * DAMAGE_INCREMENT;
        }
        public float GetThrowDistance() {
            return THROW_DISTANCE_BASE + (level - 1) * THROW_DISTANCE_INCREMENT;
        }

        public override SkillDecision GetDecision() {
            return new SkillDecision(this,
                                     "test",
                                     RangePreviewType.Ring,
                                     RANGE,
                                     RangeUtil.GetVisibleEnemiesWithinRadius(unit, RANGE));
        }
        public override void Resolve(object choice) {
            base.Resolve(choice);
            Unit target = choice as Unit;
            Vector3 directionTowardsTarget = (target.position - unit.position).normalized;
            Vector3 placementPosition = unit.position - directionTowardsTarget * GetThrowDistance();
            NavMeshHit hit;
            NavMesh.SamplePosition(placementPosition, out hit, 10f, NavMesh.AllAreas);
            if (hit.hit) {
                GameStateManagerScript.instance.EnqueueAnimation(new ThrowUnitAnimation(target, target.position, placementPosition, 4));
            }
            unit.GetAttacked(target, GetDamage());
            AfterResolve();
        }
    }
}
