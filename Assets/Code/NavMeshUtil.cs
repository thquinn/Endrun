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
        public static Vector3 GetPointAlongPath(NavMeshPath path, float percentage) {
            float length = GetPathLength(path);
            float remaining = percentage * length;
            for (int i = 0; i < path.corners.Length - 1; i++) {
                Vector2 a = new Vector2(path.corners[i].x, path.corners[i].z);
                Vector2 b = new Vector2(path.corners[i + 1].x, path.corners[i + 1].z);
                float segment = Vector2.Distance(a, b);
                if (remaining <= segment) {
                    float t = remaining / segment;
                    return Vector3.Lerp(path.corners[i], path.corners[i + 1], t);
                }
                remaining -= segment;
            }
            return path.corners[path.corners.Length - 1];
        }
        static int layerMaskChunks;
        public static Vector3 GetChunksNormal(Vector3 v) {
            if (layerMaskChunks == 0) {
                layerMaskChunks = LayerMask.GetMask(new string[] { "Chunks" });
            }
            Vector3 xyz = v + Vector3.up;
            Ray ray = new Ray(xyz, Vector3.down);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, Mathf.Infinity, layerMaskChunks);
            return hit.normal;
        }
    }
}
