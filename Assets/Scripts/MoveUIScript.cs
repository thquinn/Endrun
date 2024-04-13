using Assets.Code;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class MoveUIScript : MonoBehaviour
{
    static Vector2Int[] NEIGHBOR_DIRECTIONS = new Vector2Int[] { new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1), new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(1, -1), new Vector2Int(1, 0), new Vector2Int(1, 1) };
    static float GRID_SIZE = .5f;

    public MeshFilter meshFilter;
    public GameObject set;
    NavMeshAgent agent;
    float maxDistance;

    void Start() {
        Set(set);
    }
    void Set(GameObject moving) {
        agent = moving.GetComponent<NavMeshAgent>();
        maxDistance = 10;
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
                if (Mathf.Abs(worldSpace.x) < .1f && Mathf.Abs(worldSpace.z - 5) < .1f) {
                    Debug.Log("!");
                }
                NavMesh.SamplePosition(worldSpace, out navMeshHit, 3f, NavMesh.AllAreas);
                if (navMeshHit.hit) {
                    Vector2 xzDistance = new Vector2(worldSpace.x - navMeshHit.position.x, worldSpace.z - navMeshHit.position.z);
                    if (xzDistance.magnitude < GRID_SIZE) {
                        NavMesh.CalculatePath(agent.transform.position, navMeshHit.position, NavMesh.AllAreas, path);
                        if (NavMeshUtil.GetPathLength(path) <= maxDistance) {
                            coorsPathable.Add(neighbor, navMeshHit.position);
                            queue.Enqueue(neighbor);
                        }
                    }
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
                    triangles.Add(indices[upperLeft]);
                    triangles.Add(indices[lowerLeft]);
                    triangles.Add(indices[upperRight]);
                    if (hasLR) {
                        triangles.Add(indices[upperRight]);
                        triangles.Add(indices[lowerLeft]);
                        triangles.Add(indices[lowerRight]);
                    }
                } else {
                    if (hasUR && hasLR && hasLL) {
                        triangles.Add(indices[upperRight]);
                        triangles.Add(indices[lowerLeft]);
                        triangles.Add(indices[lowerRight]);
                    }
                    if (hasUL && hasUR && hasLR) {
                        triangles.Add(indices[upperLeft]);
                        triangles.Add(indices[lowerRight]);
                        triangles.Add(indices[upperRight]);
                    }
                    if (hasUL && hasLR && hasLL) {
                        triangles.Add(indices[upperLeft]);
                        triangles.Add(indices[lowerLeft]);
                        triangles.Add(indices[lowerRight]);
                    }
                }
            }
        }
        mesh.SetTriangles(triangles, 0);
        meshFilter.mesh = mesh;
    }

    void Update() {
        
    }
}
