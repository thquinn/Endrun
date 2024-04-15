using Assets.Code.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code
{
    public static class RangeUtil
    {
        static int layerMaskChunks;

        public static bool WithinMeleeRange(Vector3 a, Vector3 b) {
            return Vector3.Distance(a, b) <= Constants.COMBAT_MELEE_RADIUS;
        }

        public static Unit[] GetVisibleEnemiesWithinRadius(Unit unit, float radius) {
            return GetEnemies(unit).Where(u => unit.SphericalDistanceTo(u) <= radius)
                                   .Where(u => !IsTerrainBetweenUnits(unit, u)).ToArray();
        }
        public static Unit[] GetVisibleAlliesWithinRadius(Unit unit, float radius) {
            return GetAllies(unit).Where(u => unit.SphericalDistanceTo(u) <= radius)
                                  .Where(u => !IsTerrainBetweenUnits(unit, u)).ToArray();
        }
        public static Unit[] GetVisibleEnemiesWithinCone(Unit unit, float radius) {
            return GetEnemies(unit).Where(u => unit.CanShootWithinConicalDistance(u, radius))
                                   .Where(u => !IsTerrainBetweenUnits(unit, u)).ToArray();
        }
        public static IEnumerable<Unit> GetEnemies(Unit unit) {
            return unit.gameState.units.Where(u => u.playerControlled != unit.playerControlled);
        }
        public static IEnumerable<Unit> GetAllies(Unit unit) {
            return unit.gameState.units.Where(u => u.playerControlled == unit.playerControlled && u != unit);
        }

        public static bool IsTerrainBetweenUnits(Unit a, Unit b) {
            if (layerMaskChunks == 0) {
                layerMaskChunks = LayerMask.GetMask(new string[] { "Chunks" });
            }
            Vector3 delta = (b.position - a.position);
            Ray ray = new Ray(a.position + Vector3.up, delta.normalized); // add (0, 1, 0) to aim center to center
            RaycastHit hit;
            Physics.Raycast(ray, out hit, delta.magnitude, layerMaskChunks);
            return hit.collider != null;
        }

        public static Predicate<object> GetRangePredicate(Unit unit, float min, float max) {
            return (object o) => {
                if (!(o is Vector3)) return false;
                Vector3 position = (Vector3)o;
                float distance = Vector3.Distance(position, unit.position);
                bool inRange = distance >= min && distance <= max;
                bool flatGround = Vector3.Dot(Vector3.up, NavMeshUtil.GetChunksNormal(position)) > .75f;
                return inRange && flatGround;
            };
        }
    }
}
