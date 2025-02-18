using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class TileData
{
    public Vector2Int position;
    public GameObject placedObject;
}

[ExecuteInEditMode] 
public class TileGrid : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public float tileSize = 1f;
    public GameObject enemy;
    public Transform enemyPool;

    [SerializeField] public List<TileData> tileObjects = new List<TileData>(); // Visible in Inspector
    private Dictionary<Vector2Int, GameObject> tileMap = new Dictionary<Vector2Int, GameObject>();

    void OnDrawGizmos()
    {
        DrawGrid(); // Draws the grid in Scene View
    }

    void DrawGrid()
    {
        Gizmos.color = Color.gray;

        for (int x = 0; x <= width; x++)
        {
            for (int y = 0; y <= height; y++)
            {
                Vector3 worldPos = GridToWorldPosition(new Vector2Int(x, y));
                Gizmos.DrawWireCube(worldPos, Vector3.one * tileSize);
            }
        }
    }

    public Vector3 GridToWorldPosition(Vector2Int gridPos)
    {
        Vector3 offset = new Vector3(width * 0.5f * tileSize, height * 0.5f * tileSize, 0);
        Vector3 localPos = new Vector3(gridPos.x * tileSize, gridPos.y * tileSize, 0) - offset;
    
        return transform.position + transform.rotation * localPos; // Apply rotation
    }
}
