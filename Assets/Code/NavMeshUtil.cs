using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Code
{
    public static class NavMeshUtil {
        public static float GetPathLength(NavMeshPath path) {
            if (path.status != NavMeshPathStatus.PathComplete) {
                return float.PositiveInfinity;
            }
            float total = 0;
            for (int i = 0; i < path.corners.Length - 1; i++) {
                Vector2 a = new Vector2(path.corners[i].x, path.corners[i].z);
                Vector2 b = new Vector2(path.corners[i + 1].x, path.corners[i + 1].z);
                total += Vector2.Distance(a, b);
            }
            return total;
        }
        public static NavMeshHit GetNavMeshXZ(Vector2 xz) {
            Vector3 xyz = new Vector3(xz.x, 20, xz.y);
            NavMeshHit hit;
            NavMesh.Raycast(xyz, xyz + Vector3.down * 20, out hit, NavMesh.AllAreas);
            return hit;
        }
    }
}
