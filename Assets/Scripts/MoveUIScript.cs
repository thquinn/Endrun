using Assets.Code;
using Assets.Code.Animation;
using Assets.Code.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class MoveUIScript : MonoBehaviour
{
    static Vector2Int[] NEIGHBOR_DIRECTIONS = new Vector2Int[] { new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1), new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(1, -1), new Vector2Int(1, 0), new Vector2Int(1, 1) };
    static float GRID_SIZE = .33f;

    public MeshFilter meshFilter;
    public LineRenderer lineRenderer;
    public GameObject pathFinish, pathCircle;

    Unit unit;
    NavMeshAgent agent;
    float maxDistance;
    public LayerMask layerMaskChunks;

    void Start() {
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(Color.white, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1, 0.0f), new GradientAlphaKey(0, 0.8f) }
            );
        lineRenderer.colorGradient = gradient;
    }
    void Set(GameObject moving) {
        agent = moving.GetComponent<NavMeshAgent>();
        maxDistance = 7;
        // Build move preview.
        HashSet<Vector2Int> coorsSeen = new HashSet<Vector2Int>();
        Dictionary<Vector2Int, Vector3> coorsPathable = new Dictionary<Vector2Int, Vector3>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        coorsSeen.Add(Vector2Int.zero);
        coorsPathable.Add(Vector2Int.zero, agent.transform.position);
        queue.Enqueue(Vector2Int.zero);
        NavMeshPath path = new NavMeshPath();
        while (queue.Count > 0) {
            Vector2Int current = queue.Dequeue();
            foreach (Vector2Int neighborDir in NEIGHBOR_DIRECTIONS) {
                Vector2Int neighbor = current + neighborDir;
                if (coorsSeen.Contains(neighbor)) continue;
                coorsSeen.Add(neighbor);
                Vector3 worldSpace = agent.transform.position;
                worldSpace.x += neighbor.x * GRID_SIZE;
                worldSpace.z += neighbor.y * GRID_SIZE;
                NavMeshHit navMeshHit;
                NavMesh.SamplePosition(worldSpace, out navMeshHit, 3f, NavMesh.AllAreas);
                if (!navMeshHit.hit) continue;
                Vector2 xzDistance = new Vector2(worldSpace.x - navMeshHit.position.x, worldSpace.z - navMeshHit.position.z);
                if (xzDistance.magnitude > GRID_SIZE * 1.5f) continue;
                NavMesh.CalculatePath(agent.transform.position, navMeshHit.position, NavMesh.AllAreas, path);
                if (Mathf.Abs(navMeshHit.position.x) < .1f && Mathf.Abs(navMeshHit.position.z - 3) < .1f) {
                    Debug.Log("!");
                }
                if (path.status != NavMeshPathStatus.PathComplete) continue;
                if (NavMeshUtil.GetPathLength(path) <= maxDistance) {
                    coorsPathable.Add(neighbor, navMeshHit.position);
                    queue.Enqueue(neighbor);
                }
            }
        }
        SetPreviewMesh(coorsPathable);
    }
    void SetPreviewMesh(Dictionary<Vector2Int, Vector3> coors) {
        Vector3[] vertices = new Vector3[coors.Count];
        Dictionary<Vector2Int, int> indices = new Dictionary<Vector2Int, int>();
        foreach (var kvp in coors) {
            vertices[indices.Count] = kvp.Value;
            indices.Add(kvp.Key, indices.Count);
        }
        Mesh mesh = new Mesh();
        mesh.SetVertices(vertices);
        // Create triangles.
        int minX = coors.Keys.Min(c => c.x);
        int maxX = coors.Keys.Max(c => c.x);
        int minY = coors.Keys.Min(c => c.y);
        int maxY = coors.Keys.Max(c => c.y);
        List<int> triangles = new List<int>();
        for (int x = minX; x < maxX; x++) {
            for (int y = minY; y < maxY; y++) {
                Vector2Int upperLeft = new Vector2Int(x, y);
                Vector2Int upperRight = new Vector2Int(x + 1, y);
                Vector2Int lowerLeft = new Vector2Int(x, y + 1);
                Vector2Int lowerRight = new Vector2Int(x + 1, y + 1);
                bool hasUL = indices.ContainsKey(upperLeft);
                bool hasUR = indices.ContainsKey(upperRight);
                bool hasLL = indices.ContainsKey(lowerLeft);
                bool hasLR = indices.ContainsKey(lowerRight);
                if (hasUL && hasUR && hasLL) {
                    AddTriangleIfNormalAligned(coors, triangles, indices, upperLeft, lowerLeft, upperRight);
                    if (hasLR) {
                        AddTriangleIfNormalAligned(coors, triangles, indices, upperRight, lowerLeft, lowerRight);
                    }
                } else {
                    if (hasUR && hasLR && hasLL) {
                        AddTriangleIfNormalAligned(coors, triangles, indices, upperRight, lowerLeft, lowerRight);
                    }
                    if (hasUL && hasUR && hasLR) {
                        AddTriangleIfNormalAligned(coors, triangles, indices, upperLeft, lowerRight, upperRight);
                    }
                    if (hasUL && hasLR && hasLL) {
                        AddTriangleIfNormalAligned(coors, triangles, indices, upperLeft, lowerLeft, lowerRight);
                    }
                }
            }
        }
        mesh.SetTriangles(triangles, 0);
        meshFilter.mesh = mesh;
    }
    void AddTriangleIfNormalAligned(Dictionary<Vector2Int, Vector3> coors, List<int> triangles, Dictionary<Vector2Int, int> indices, Vector2Int a, Vector2Int b, Vector2Int c) {
        int iA = indices[a], iB = indices[b], iC = indices[c];
        // This is to avoid making step (i.e. stepping up onto a ledge) previews look terrible; let's just not have any stepping.
        /*
        Vector3 vA = coors[a], vB = coors[b], vC = coors[c];
        Vector3 center = (vA + vB + vC) / 3;
        Vector3 chunksNormal = NavMeshUtil.GetChunksNormal(center);
        Vector3 triangleNormal = Vector3.Cross(vB - vA, vC - vA).normalized;
        if (Vector3.Dot(chunksNormal, triangleNormal) > .95f) {
            triangles.Add(iA);
            triangles.Add(iB);
            triangles.Add(iC);
        }
        */
        triangles.Add(iA);
        triangles.Add(iB);
        triangles.Add(iC);
    }

    void Update() {
        if (GameStateManagerScript.instance.activeUnit != unit) {
            unit = GameStateManagerScript.instance.activeUnit;
            if (unit != null) {
                Set(GameStateManagerScript.instance.unitScripts[unit].gameObject);
            }
        }
        if (unit == null) {
            return;
        }

        Vector3 target = Util.GetMouseCollisionPoint(layerMaskChunks);
        NavMeshPath path = null;
        if (target == Vector3.zero) {
            ClearPathPreview();
        } else {
            path = new NavMeshPath();
            NavMesh.CalculatePath(agent.transform.position, target, NavMesh.AllAreas, path);
            if (NavMeshUtil.GetPathLength(path) > maxDistance) {
                ClearPathPreview();
            } else {
                float displayRadius = Mathf.Min(Vector3.Distance(path.corners[0], path.corners[1]), 1f);
                path.corners[0] = Vector3.MoveTowards(path.corners[0], path.corners[1], displayRadius);
                lineRenderer.positionCount = path.corners.Length;
                lineRenderer.SetPositions(path.corners);
                Vector3 pathEnd = path.corners[path.corners.Length - 1];
                pathFinish.SetActive(true);
                pathFinish.transform.position = pathEnd;
                pathCircle.SetActive(true);
                pathCircle.transform.rotation = Quaternion.LookRotation(NavMeshUtil.GetChunksNormal(pathEnd));
                pathCircle.transform.localScale = new Vector3(agent.radius * 2, agent.radius * 2, 1);
            }
        }
        if (path != null && Input.GetMouseButtonDown(0)) {
            GameStateManagerScript.instance.EnqueueAnimation(new MoveAnimation(unit, path));
        }
    }
    void ClearPathPreview() {
        lineRenderer.positionCount = 0;
        pathFinish.SetActive(false);
        pathCircle.SetActive(false);
    }
}
