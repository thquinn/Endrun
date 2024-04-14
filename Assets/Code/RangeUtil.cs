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

        public static Unit[] GetVisibleEnemiesWithinRadius(Unit unit, float radius) {
            return GetEnemies(unit).Where(u => unit.DistanceTo(u) <= radius)
                                   .Where(u => !IsTerrainBetweenUnits(unit, u)).ToArray();
        }
        public static IEnumerable<Unit> GetEnemies(Unit unit) {
            return unit.gameState.units.Where(u => u.playerControlled != unit.playerControlled);
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
    }
}
